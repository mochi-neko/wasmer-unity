using System;
using System.Runtime.InteropServices;

namespace Mochineko.WasmerBridge
{
    public sealed class Module : IDisposable
    {
        public string Name { get; }
        
        private readonly NativeHandle handle;

        internal NativeHandle Handle
        {
            get
            {
                if (handle.IsInvalid)
                {
                    throw new ObjectDisposedException(typeof(Module).FullName);
                }

                return handle;
            }
        }

        // private readonly ImportType[] importTypes;
        // private readonly ExportType[] exportTypes;
        
        public static bool Validate(Store store, NativeByteArray binary)
        {
            if (store is null)
            {
                throw new ArgumentNullException(nameof(store));
            }
            
            if (binary.size == 0)
            {
                throw new ArgumentNullException(nameof(binary));
            }

            return WasmAPIs.wasm_module_validate(store.Handle, binary);
        }

        public Module(Store store, string name, NativeByteArray binary)
        {
            if (store is null)
            {
                throw new ArgumentNullException(nameof(store));
            }

            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (binary.size == 0)
            {
                throw new ArgumentNullException(nameof(binary));
            }

            var moduleHandle = WasmAPIs.wasm_module_new(store.Handle, binary);
            if (moduleHandle == IntPtr.Zero)
            {
                Marshal.FreeHGlobal(moduleHandle);
                throw new InvalidOperationException("Failed to create module.");
            }

            this.Name = name;
            this.handle = new NativeHandle(moduleHandle);

            // WasmAPIs.wasm_module_imports(handle, out var imports);
            // using (var _ = imports)
            // {
            //     this.importTypes = imports.ToImportArray();
            // }
            //
            // WasmAPIs.wasm_module_exports(handle, out var exports);
            //
            // using (var _ = exports)
            // {
            //     this.exportTypes = exports.ToExportArray();
            // }
        }

        public void Dispose()
        {
            handle.Dispose();
        }

        internal sealed class NativeHandle : SafeHandle
        {
            public NativeHandle(IntPtr handle)
                : base(IntPtr.Zero,true)
            {
                SetHandle(handle);
            }

            public override bool IsInvalid
                => handle == IntPtr.Zero;

            protected override bool ReleaseHandle()
            {
                WasmAPIs.wasm_module_delete(handle);
                return true;
            }
        }

        private static class WasmAPIs
        {
            [DllImport(NativePlugin.LibraryName)]
            public static extern bool wasm_module_validate(Store.NativeHandle store, in NativeByteArray binary);
            
            [DllImport(NativePlugin.LibraryName)]
            public static extern IntPtr wasm_module_new(Store.NativeHandle store, in NativeByteArray binary);

            // TODO:
            //[DllImport(NativePlugin.LibraryName)]
            //public static extern void wasm_module_imports(NativeHandle module, out ImportTypeArray importTypes);
            
            // TODO:
            //[DllImport(NativePlugin.LibraryName)]
            //public static extern void wasm_module_exports(NativeHandle module, out ExportTypeArray exportTypes);
            
            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_module_delete(IntPtr module);
        }
    }
}