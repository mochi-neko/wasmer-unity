using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Mochineko.WasmerUnity.Wasm.Attributes;
using Microsoft.Win32.SafeHandles;

namespace Mochineko.WasmerUnity.Wasm
{
    [OwnPointed]
    public sealed class Instance : IDisposable
    {
        private IReadOnlyDictionary<string, ExternalInstance> nameToInstanceMap;
        private IDisposable destructor;

        internal void Exports([OwnOut] out ExternalInstanceVector exports)
            => WasmAPIs.wasm_instance_exports(Handle, out exports);

        public FunctionInstance GetFunction(Store store, string name)
        {
            if (!nameToInstanceMap.TryGetValue(name, out var externalInstance))
            {
                throw new KeyNotFoundException(nameof(name));
            }

            if (externalInstance.Kind != ExternalKind.Function)
            {
                // TODO:
                throw new Exception();
            }

            return externalInstance.ToFunction();
        }

        public GlobalInstance GetGlobal(string name)
        {
            if (!nameToInstanceMap.TryGetValue(name, out var externalInstance))
            {
                throw new KeyNotFoundException(nameof(name));
            }

            if (externalInstance.Kind != ExternalKind.Global)
            {
                // TODO:
                throw new Exception();
            }

            return externalInstance.ToGlobal();
        }

        [return: OwnReceive]
        public static Instance New(Store store, Module module, ImportObject importObject)
        {
            importObject.GetImports(out var imports);

            TrapPointer.New(store, out var trapPointer);

            var instance = new Instance(
                WasmAPIs.wasm_instance_new(store.Handle, module.Handle, in imports, in trapPointer),
                hasOwnership: true);

            // TODO: Error handling by TrapPointer
            
            var construction = ConstructNameToExternalInstanceMap(module, instance);
            instance.nameToInstanceMap = construction.map;
            instance.destructor = construction.destructor;

            return instance;
        }

        [return: OwnReceive]
        internal static Instance New(Store store, Module module, in ExternalInstanceVector imports)
        {
            TrapPointer.New(store, out var trapPointer);

            var instance = new Instance(
                WasmAPIs.wasm_instance_new(store.Handle, module.Handle, in imports, in trapPointer),
                hasOwnership: true);
            
            // TODO: Error handling by TrapPointer

            var construction = ConstructNameToExternalInstanceMap(module, instance);
            instance.nameToInstanceMap = construction.map;
            instance.destructor = construction.destructor;

            return instance;
        }

        private static (IReadOnlyDictionary<string, ExternalInstance> map, IDisposable destructor)
            ConstructNameToExternalInstanceMap(Module module, Instance instance)
        {
            var nameToInstanceMap = new Dictionary<string, ExternalInstance>();

            module.Exports(out var exportTypes);
            using (exportTypes)
            {
                instance.Exports(out var exportInstances);
                for (var i = 0; i < (int)exportInstances.size; i++)
                {
                    unsafe
                    {
                        using var exportType = 
                            ExportType.FromPointer(exportTypes.data[i], hasOwnership: false);
                        var externalInstance =
                            ExternalInstance.FromPointer(exportInstances.data[i], hasOwnership: false);

                        if (exportType.Kind != externalInstance.Kind)
                        {
                            // TODO:
                            throw new Exception();
                        }
                        
                        nameToInstanceMap.Add(exportType.Name, externalInstance);
                    }
                }
                
                return (nameToInstanceMap, exportInstances);
            }
        }

        private Instance(IntPtr handle, bool hasOwnership)
        {
            this.handle = new NativeHandle(handle, hasOwnership);
        }

        public void Dispose()
        {
            handle.Dispose();
            destructor?.Dispose();
        }

        private readonly NativeHandle handle;

        internal NativeHandle Handle
        {
            get
            {
                if (handle.IsInvalid)
                {
                    throw new ObjectDisposedException(typeof(Instance).FullName);
                }

                return handle;
            }
        }

        internal sealed class NativeHandle : SafeHandleZeroOrMinusOneIsInvalid
        {
            public NativeHandle(IntPtr handle, bool ownsHandle)
                : base(ownsHandle)
            {
                SetHandle(handle);
            }

            protected override bool ReleaseHandle()
            {
                WasmAPIs.wasm_instance_delete(handle);
                return true;
            }
        }

        private static class WasmAPIs
        {
            [DllImport(NativePlugin.LibraryName)]
            [return: OwnReceive]
            public static extern IntPtr wasm_instance_new(
                Store.NativeHandle store,
                [Const] Module.NativeHandle module,
                [ConstVector] in ExternalInstanceVector imports,
                [OwnPass] in TrapPointer trapPinter);

            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_instance_delete(
                [OwnPass] [In] IntPtr handle);

            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_instance_exports(
                [Const] NativeHandle handle,
                [OwnOut] out ExternalInstanceVector exports);
        }
    }
}