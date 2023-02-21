using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using Mochineko.WasmerBridge.Attributes;

namespace Mochineko.WasmerBridge
{
    [OwnPointed]
    internal sealed class ExternalInstance : IDisposable
    {
        internal ExternalKind Kind
            => (ExternalKind)WasmAPIs.wasm_extern_kind(Handle);

        internal ExternalType Type
            => ExternalType.FromPointer(WasmAPIs.wasm_extern_type(Handle));

        private ExternalInstance(IntPtr handle)
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
                    throw new ObjectDisposedException(typeof(ExternalInstance).FullName);
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
            public static extern void wasm_extern_delete(
                [OwnPass] [In] IntPtr external);

            [DllImport(NativePlugin.LibraryName)]
            [return: OwnReceive]
            public static extern IntPtr wasm_extern_copy(
                [Const] NativeHandle external);

            [DllImport(NativePlugin.LibraryName)]
            public static extern bool wasm_extern_same(
                [Const] NativeHandle left,
                [Const] NativeHandle right);

            [DllImport(NativePlugin.LibraryName)]
            public static extern byte wasm_extern_kind(
                [Const] NativeHandle external);

            [DllImport(NativePlugin.LibraryName)]
            [return: OwnReceive]
            public static extern IntPtr wasm_extern_type(
                [Const] NativeHandle external);

            [DllImport(NativePlugin.LibraryName)]
            public static extern IntPtr wasm_func_as_extern(FunctionInstance.NativeHandle function);

            // [DllImport(NativePlugin.LibraryName)]
            // public static extern IntPtr wasm_global_as_extern(GlobalInstance.NativeHandle global);
            //
            // [DllImport(NativePlugin.LibraryName)]
            // public static extern IntPtr wasm_table_as_extern(TableInstance.NativeHandle table);
            //
            // [DllImport(NativePlugin.LibraryName)]
            // public static extern IntPtr wasm_memory_as_extern(MemoryInstance.NativeHandle memory);

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
            public static extern IntPtr wasm_func_as_extern_const([Const] IntPtr function);

            [DllImport(NativePlugin.LibraryName)]
            [return: Const]
            public static extern IntPtr wasm_global_as_extern_const([Const] IntPtr global);

            [DllImport(NativePlugin.LibraryName)]
            [return: Const]
            public static extern IntPtr wasm_table_as_extern_const([Const] IntPtr table);

            [DllImport(NativePlugin.LibraryName)]
            [return: Const]
            public static extern IntPtr wasm_memory_as_extern_const([Const] IntPtr memory);

            [DllImport(NativePlugin.LibraryName)]
            [return: Const]
            public static extern IntPtr wasm_extern_as_func_const([Const] NativeHandle external);

            [DllImport(NativePlugin.LibraryName)]
            [return: Const]
            public static extern IntPtr wasm_extern_as_global_const([Const] NativeHandle external);

            [DllImport(NativePlugin.LibraryName)]
            [return: Const]
            public static extern IntPtr wasm_extern_as_table_const([Const] NativeHandle external);

            [DllImport(NativePlugin.LibraryName)]
            [return: Const]
            public static extern IntPtr wasm_extern_as_memory_const([Const] NativeHandle external);
        }
    }
}