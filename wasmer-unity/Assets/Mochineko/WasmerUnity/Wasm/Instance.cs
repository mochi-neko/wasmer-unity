using System;
using System.Runtime.InteropServices;
using Mochineko.WasmerUnity.Wasm.Attributes;
using Microsoft.Win32.SafeHandles;

namespace Mochineko.WasmerUnity.Wasm
{
    [OwnPointed]
    public sealed class Instance : IDisposable
    {
        internal void Exports([OwnOut] out ExternalInstanceVector exports)
            => WasmAPIs.wasm_instance_exports(Handle, out exports);

        [return: OwnReceive]
        internal static Instance New(Store store, Module module, in ExternalInstanceVector imports)
        {
            TrapPointer.New(store, out var trapPointer);

            return new Instance(
                WasmAPIs.wasm_instance_new(store.Handle, module.Handle, in imports, in trapPointer),
                hasOwnership: true);

            // TODO: Error handling by TrapPointer
        }

        private Instance(IntPtr handle, bool hasOwnership)
        {
            this.handle = new NativeHandle(handle, hasOwnership);
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