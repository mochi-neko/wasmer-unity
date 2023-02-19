using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using Mochineko.WasmerBridge.Attributes;

namespace Mochineko.WasmerBridge
{
    [OwnPointed]
    internal sealed class ImportType : IDisposable
    {
        private ImportType(IntPtr handle)
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
                    throw new ObjectDisposedException(typeof(ImportType).FullName);
                }

                return handle;
            }
        }

        internal sealed class NativeHandle : SafeHandleZeroOrMinusOneIsInvalid
        {
            public NativeHandle(IntPtr handle) : base(true)
            {
                this.handle = handle;
            }

            protected override bool ReleaseHandle()
            {
                WasmAPIs.wasm_importtype_delete(handle);
                return true;
            }
        }

        private static class WasmAPIs
        {
            [DllImport(NativePlugin.LibraryName)]
            [return: OwnReceive]
            public static extern IntPtr wasm_importtype_new(
                [OwnPass] Module.NativeHandle module,
                [OwnPass] in ByteVector name,
                [OwnPass] in ExternalType externalType);
            
            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_importtype_delete([OwnPass] IntPtr external);

            [DllImport(NativePlugin.LibraryName)]
            [return: OwnReceive]
            public static extern IntPtr wasm_importtype_copy([Const] NativeHandle external);

            [DllImport(NativePlugin.LibraryName)]
            public static extern bool wasm_importtype_same([Const] NativeHandle left, [Const] NativeHandle right);
            
            [DllImport(NativePlugin.LibraryName)]
            [return: Const]
            public static extern unsafe ByteVector* wasm_importtype_module([Const] NativeHandle importType);

            [DllImport(NativePlugin.LibraryName)]
            [return: Const]
            public static extern IntPtr wasm_importtype_name([Const] NativeHandle importType);

            [DllImport(NativePlugin.LibraryName)]
            [return: Const]
            public static extern IntPtr wasm_importtype_type([Const] NativeHandle importType);
        }
    }
}