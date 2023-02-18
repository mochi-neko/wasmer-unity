using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using Mochineko.WasmerBridge.Attributes;

namespace Mochineko.WasmerBridge
{
    [OwnPointed]
    internal sealed class External : IDisposable
    {
        // NOTE: Externals only get from each external kind instance
        private External(IntPtr handle)
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
                    throw new ObjectDisposedException(typeof(External).FullName);
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
                WasmAPIs.wasm_extern_delete(handle);
                return true;
            }
        }

        private static class WasmAPIs
        {
            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_extern_delete([OwnPass] IntPtr external);
            
            [DllImport(NativePlugin.LibraryName)]
            [return: OwnReceive]
            public static extern IntPtr wasm_extern_copy([Const] NativeHandle external);

            [DllImport(NativePlugin.LibraryName)]
            public static extern bool wasm_extern_same([Const] NativeHandle left, [Const] NativeHandle right);
            
            [DllImport(NativePlugin.LibraryName)]
            public static extern ExternalKind wasm_extern_kind([Const] NativeHandle external);

            [DllImport(NativePlugin.LibraryName)]
            [return: OwnReceive]
            public static extern IntPtr wasm_extern_type([Const] NativeHandle external);

            [DllImport(NativePlugin.LibraryName)]
            public static extern IntPtr wasm_func_as_extern(IntPtr function);

            [DllImport(NativePlugin.LibraryName)]
            public static extern IntPtr wasm_global_as_extern(IntPtr global);

            [DllImport(NativePlugin.LibraryName)]
            public static extern IntPtr wasm_table_as_extern(IntPtr table);

            [DllImport(NativePlugin.LibraryName)]
            public static extern IntPtr wasm_memory_as_extern(IntPtr memory);
            
            [DllImport(NativePlugin.LibraryName)]
            public static extern IntPtr wasm_extern_as_func(NativeHandle external);

            [DllImport(NativePlugin.LibraryName)]
            public static extern IntPtr wasm_extern_as_global(NativeHandle external);

            [DllImport(NativePlugin.LibraryName)]
            public static extern IntPtr wasm_extern_as_table(NativeHandle external);

            [DllImport(NativePlugin.LibraryName)]
            public static extern IntPtr wasm_extern_as_memory(NativeHandle external);

            [DllImport(NativePlugin.LibraryName)]
            [return: Const]
            public static extern IntPtr wasm_func_as_extern_const([Const]IntPtr function);

            [DllImport(NativePlugin.LibraryName)]
            [return: Const]
            public static extern IntPtr wasm_global_as_extern_const([Const]IntPtr global);

            [DllImport(NativePlugin.LibraryName)]
            [return: Const]
            public static extern IntPtr wasm_table_as_extern_const([Const]IntPtr table);

            [DllImport(NativePlugin.LibraryName)]
            [return: Const]
            public static extern IntPtr wasm_memory_as_extern_const([Const]IntPtr memory);
            
            [DllImport(NativePlugin.LibraryName)]
            [return: Const]
            public static extern IntPtr wasm_extern_as_func_const([Const]NativeHandle external);

            [DllImport(NativePlugin.LibraryName)]
            [return: Const]
            public static extern IntPtr wasm_extern_as_global_const([Const]NativeHandle external);

            [DllImport(NativePlugin.LibraryName)]
            [return: Const]
            public static extern IntPtr wasm_extern_as_table_const([Const]NativeHandle external);

            [DllImport(NativePlugin.LibraryName)]
            [return: Const]
            public static extern IntPtr wasm_extern_as_memory_const([Const]NativeHandle external);
        }
    }
}