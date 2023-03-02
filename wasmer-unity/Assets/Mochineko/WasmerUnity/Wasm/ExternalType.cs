using System;
using System.Runtime.InteropServices;
using Mochineko.WasmerUnity.Wasm.Attributes;
using Microsoft.Win32.SafeHandles;

namespace Mochineko.WasmerUnity.Wasm
{
    [OwnPointed]
    internal sealed class ExternalType : IDisposable
    {
        internal ExternalKind Kind
            => (ExternalKind)WasmAPIs.wasm_externtype_kind(Handle);

        internal static ExternalType FromFunction(FunctionType function)
            => new ExternalType(
                WasmAPIs.wasm_functype_as_externtype(function.Handle),
                hasOwnership: false);

        internal FunctionType ToFunction()
            => FunctionType.FromPointer(
                WasmAPIs.wasm_externtype_as_functype(Handle),
                hasOwnership: false);

        internal static ExternalType FromGlobal(GlobalType global)
            => new ExternalType(
                WasmAPIs.wasm_globaltype_as_externtype(global.Handle),
                hasOwnership: false);

        internal GlobalType ToGlobal()
            => GlobalType.FromPointer(
                WasmAPIs.wasm_externtype_as_globaltype(Handle),
                hasOwnership: false);

        internal static ExternalType FromMemory(MemoryType memory)
            => new ExternalType(
                WasmAPIs.wasm_memorytype_as_externtype(memory.Handle),
                hasOwnership: false);

        internal MemoryType ToMemory()
            => MemoryType.FromPointer(
                WasmAPIs.wasm_externtype_as_memorytype(Handle),
                hasOwnership: false);

        internal static ExternalType FromPointer(IntPtr pointer, bool hasOwnership)
            => new ExternalType(pointer, hasOwnership);

        private ExternalType(IntPtr handle, bool hasOwnership)
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
                    throw new ObjectDisposedException(typeof(ExternalType).FullName);
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
                WasmAPIs.wasm_externtype_delete(handle);
                return true;
            }
        }

        private static class WasmAPIs
        {
            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_externtype_delete(
                [OwnPass] [In] IntPtr handle);

            [DllImport(NativePlugin.LibraryName)]
            public static extern byte wasm_externtype_kind(
                [Const] NativeHandle handle);

            [DllImport(NativePlugin.LibraryName)]
            public static extern IntPtr wasm_functype_as_externtype(
                FunctionType.NativeHandle function);

            [DllImport(NativePlugin.LibraryName)]
            public static extern IntPtr wasm_globaltype_as_externtype(
                GlobalType.NativeHandle global);

            //TODO:
            // [DllImport(NativePlugin.LibraryName)]
            // public static extern IntPtr wasm_tabletype_as_externtype(TableType.NativeHandle functionType);

            [DllImport(NativePlugin.LibraryName)]
            public static extern IntPtr wasm_memorytype_as_externtype(
                MemoryType.NativeHandle memory);

            [DllImport(NativePlugin.LibraryName)]
            public static extern IntPtr wasm_externtype_as_functype(
                NativeHandle handle);

            [DllImport(NativePlugin.LibraryName)]
            public static extern IntPtr wasm_externtype_as_globaltype(
                NativeHandle handle);

            [DllImport(NativePlugin.LibraryName)]
            public static extern IntPtr wasm_externtype_as_tabletype(
                NativeHandle handle);

            [DllImport(NativePlugin.LibraryName)]
            public static extern IntPtr wasm_externtype_as_memorytype(
                NativeHandle handle);

            [DllImport(NativePlugin.LibraryName)]
            [return: Const]
            public static extern IntPtr wasm_functype_as_externtype_const(
                [Const] FunctionType.NativeHandle function);

            [DllImport(NativePlugin.LibraryName)]
            [return: Const]
            public static extern IntPtr wasm_globaltype_as_externtype_const(
                [Const] GlobalType.NativeHandle global);

            // [DllImport(NativePlugin.LibraryName)]
            // [return: Const]
            // public static extern IntPtr wasm_tabletype_as_externtype_const([Const] TableType.NativeHandle functionType);
            //
            [DllImport(NativePlugin.LibraryName)]
            [return: Const]
            public static extern IntPtr wasm_memorytype_as_externtype_const(
                [Const] MemoryType.NativeHandle memory);

            [DllImport(NativePlugin.LibraryName)]
            [return: Const]
            public static extern IntPtr wasm_externtype_as_functype_const(
                [Const] NativeHandle handle);

            [DllImport(NativePlugin.LibraryName)]
            [return: Const]
            public static extern IntPtr wasm_externtype_as_globaltype_const(
                [Const] NativeHandle handle);

            [DllImport(NativePlugin.LibraryName)]
            [return: Const]
            public static extern IntPtr wasm_externtype_as_tabletype_const(
                [Const] NativeHandle handle);

            [DllImport(NativePlugin.LibraryName)]
            [return: Const]
            public static extern IntPtr wasm_externtype_as_memorytype_const(
                [Const] NativeHandle handle);
        }
    }
}