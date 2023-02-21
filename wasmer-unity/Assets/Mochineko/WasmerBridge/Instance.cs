using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using Mochineko.WasmerBridge.Attributes;

namespace Mochineko.WasmerBridge
{
    [OwnPointed]
    public sealed class Instance : IDisposable
    {
        internal static unsafe Instance New(Store store, Module module, in ExternalVector imports, in Trap trap)
        {
            // TODO:
            //return new Instance(WasmAPIs.wasm_instance_new(store.Handle, module.Handle, in imports, &trap);
            throw new NotImplementedException();
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
            public static extern unsafe void wasm_instance_new(
                Store.NativeHandle store,
                [Const] Module.NativeHandle module,
                [ConstVector] in ExternalVector imports,
                [OwnPass] [In] Trap.NativeHandle trap);

            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_instance_delete(
                [OwnPass] [In] IntPtr instance);

            [DllImport(NativePlugin.LibraryName)]
            public static extern unsafe void wasm_instance_exports(
                [Const] NativeHandle instance,
                [OwnOut] [Out] out ExternalVector externalVector);
        }
    }
}