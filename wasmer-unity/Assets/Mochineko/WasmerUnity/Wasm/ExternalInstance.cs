using System;
using System.Runtime.InteropServices;
using Mochineko.WasmerUnity.Wasm.Attributes;
using Microsoft.Win32.SafeHandles;

namespace Mochineko.WasmerUnity.Wasm
{
    [OwnPointed]
    internal sealed class ExternalInstance : IDisposable
    {
        internal ExternalKind Kind
            => (ExternalKind)WasmAPIs.wasm_extern_kind(Handle);

        [OwnReceive]
        internal ExternalType Type
            => ExternalType.FromPointer(
                WasmAPIs.wasm_extern_type(Handle),
                hasOwnership: true);

        // Pass ownership because ExternalInstanceVector releases not "FunctionInstance" but "ExternalInstance".
        [return: OwnReceive]
        internal static ExternalInstance FromFunctionWithOwnership([OwnPass] FunctionInstance instance)
        {
            var externalInstance = new ExternalInstance(
                WasmAPIs.wasm_func_as_extern(instance.Handle),
                hasOwnership: true);

            // Passes ownership from FunctionInstance to ExternalInstance
            instance.Handle.SetHandleAsInvalid();

            return externalInstance;
        }

        internal static ExternalInstance FromFunction(FunctionInstance instance)
        {
            return new ExternalInstance(
                WasmAPIs.wasm_func_as_extern(instance.Handle),
                hasOwnership: true);
        }

        internal FunctionInstance ToFunction()
        {
            return FunctionInstance.FromPointer(
                WasmAPIs.wasm_extern_as_func(Handle),
                hasOwnership: false);
        }

        // Pass ownership because ExternalInstanceVector releases not "GlobalInstance" but "ExternalInstance".
        [return: OwnReceive]
        internal static ExternalInstance FromGlobalWithOwnership([OwnPass] GlobalInstance instance)
        {
            var externalInstance = new ExternalInstance(
                WasmAPIs.wasm_global_as_extern(instance.Handle),
                hasOwnership: true);

            // Passes ownership from GlobalInstance to ExternalInstance
            instance.Handle.SetHandleAsInvalid();

            return externalInstance;
        }

        internal static ExternalInstance FromGlobal(GlobalInstance instance)
        {
            return new ExternalInstance(
                WasmAPIs.wasm_global_as_extern(instance.Handle),
                hasOwnership: true);
        }

        internal GlobalInstance ToGlobal()
        {
            return GlobalInstance.FromPointer(
                WasmAPIs.wasm_extern_as_global(Handle),
                hasOwnership: false);
        }

        internal static ExternalInstance FromPointer(IntPtr ptr, bool hasOwnership)
            => new ExternalInstance(ptr, hasOwnership);

        private ExternalInstance(IntPtr handle, bool hasOwnership)
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
                    throw new ObjectDisposedException(typeof(ExternalInstance).FullName);
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
                WasmAPIs.wasm_extern_delete(handle);
                return true;
            }
        }

        private static class WasmAPIs
        {
            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_extern_delete(
                [OwnPass] [In] IntPtr handle);

            [DllImport(NativePlugin.LibraryName)]
            public static extern bool wasm_extern_same(
                [Const] NativeHandle left,
                [Const] NativeHandle right);

            [DllImport(NativePlugin.LibraryName)]
            public static extern byte wasm_extern_kind(
                [Const] NativeHandle handle);

            [DllImport(NativePlugin.LibraryName)]
            [return: OwnReceive]
            public static extern IntPtr wasm_extern_type(
                [Const] NativeHandle handle);

            [DllImport(NativePlugin.LibraryName)]
            public static extern IntPtr wasm_func_as_extern(
                FunctionInstance.NativeHandle function);

            [DllImport(NativePlugin.LibraryName)]
            public static extern IntPtr wasm_global_as_extern(
                GlobalInstance.NativeHandle global);

            // TODO:
            // [DllImport(NativePlugin.LibraryName)]
            // public static extern IntPtr wasm_table_as_extern(TableInstance.NativeHandle table);
            //
            // [DllImport(NativePlugin.LibraryName)]
            // public static extern IntPtr wasm_memory_as_extern(MemoryInstance.NativeHandle memory);

            [DllImport(NativePlugin.LibraryName)]
            public static extern IntPtr wasm_extern_as_func(
                NativeHandle external);

            [DllImport(NativePlugin.LibraryName)]
            public static extern IntPtr wasm_extern_as_global(
                NativeHandle external);

            [DllImport(NativePlugin.LibraryName)]
            public static extern IntPtr wasm_extern_as_table(
                NativeHandle external);

            [DllImport(NativePlugin.LibraryName)]
            public static extern IntPtr wasm_extern_as_memory(
                NativeHandle external);

            [DllImport(NativePlugin.LibraryName)]
            [return: Const]
            public static extern IntPtr wasm_func_as_extern_const(
                [Const] IntPtr function);

            [DllImport(NativePlugin.LibraryName)]
            [return: Const]
            public static extern IntPtr wasm_global_as_extern_const(
                [Const] IntPtr global);

            [DllImport(NativePlugin.LibraryName)]
            [return: Const]
            public static extern IntPtr wasm_table_as_extern_const(
                [Const] IntPtr table);

            [DllImport(NativePlugin.LibraryName)]
            [return: Const]
            public static extern IntPtr wasm_memory_as_extern_const(
                [Const] IntPtr memory);

            [DllImport(NativePlugin.LibraryName)]
            [return: Const]
            public static extern IntPtr wasm_extern_as_func_const(
                [Const] NativeHandle external);

            [DllImport(NativePlugin.LibraryName)]
            [return: Const]
            public static extern IntPtr wasm_extern_as_global_const(
                [Const] NativeHandle external);

            [DllImport(NativePlugin.LibraryName)]
            [return: Const]
            public static extern IntPtr wasm_extern_as_table_const(
                [Const] NativeHandle external);

            [DllImport(NativePlugin.LibraryName)]
            [return: Const]
            public static extern IntPtr wasm_extern_as_memory_const(
                [Const] NativeHandle external);
        }
    }
}