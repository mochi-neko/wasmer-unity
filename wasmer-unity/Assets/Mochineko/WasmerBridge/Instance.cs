using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using Mochineko.WasmerBridge.Attributes;

namespace Mochineko.WasmerBridge
{
    [OwnPointed]
    public sealed class Instance : IDisposable
    {
        internal void Exports(out ExternalVector exports)
        {
            WasmAPIs.wasm_instance_exports(Handle, out exports);
        }

        internal static Instance New(Store store, Module module, in ExternalVector imports)
        {
            TrapPointer.New(store, out var trapPointer);
            
            return new Instance(WasmAPIs.wasm_instance_new(store.Handle, module.Handle, in imports, in trapPointer));
            
            // TODO: Error handling by TrapPointer
        }

        private Instance(IntPtr handle)
        {
            this.handle = new NativeHandle(handle);
        }

        public void Dispose()
        {
            handle.Dispose();
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
            public NativeHandle(IntPtr handle)
                : base(true)
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
                [ConstVector] in ExternalVector imports,
                [OwnPass] in TrapPointer trapPinter);

            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_instance_delete(
                [OwnPass] [In] IntPtr instance);

            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_instance_exports(
                [Const] NativeHandle instance,
                [OwnOut] [Out] out ExternalVector exports);
        }
    }
}