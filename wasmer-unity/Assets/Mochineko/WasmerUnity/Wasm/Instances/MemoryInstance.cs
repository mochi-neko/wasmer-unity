using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using Mochineko.WasmerUnity.Wasm.Attributes;
using Mochineko.WasmerUnity.Wasm.Types;

namespace Mochineko.WasmerUnity.Wasm.Instances
{
    [OwnPointed]
    public sealed class MemoryInstance : IDisposable
    {
        [OwnReceive]
        public MemoryType Type
            => MemoryType.FromPointer(
                WasmAPIs.wasm_memory_type(Handle),
                hasOwnership: true);

        internal unsafe byte* Data
            => WasmAPIs.wasm_memory_data(Handle);

        public nuint DataSize
            => WasmAPIs.wasm_memory_data_size(Handle);

        public uint Size
            => WasmAPIs.wasm_memory_size(Handle);

        public bool Grow(uint delta)
            => WasmAPIs.wasm_memory_grow(Handle, delta);

        public const nuint MemoryPageSize = 0x10000;

        public static MemoryInstance New(Store store, MemoryType type)
            => new MemoryInstance(
                WasmAPIs.wasm_memory_new(store.Handle, type.Handle),
                hasOwnership: true);

        internal static MemoryInstance FromPointer(IntPtr ptr, bool hasOwnership)
            => new MemoryInstance(ptr, hasOwnership);

        private MemoryInstance(IntPtr handle, bool hasOwnership)
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
                    throw new ObjectDisposedException(typeof(MemoryInstance).FullName);
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
                WasmAPIs.wasm_memory_delete(handle);
                return true;
            }
        }

        private static class WasmAPIs
        {
            [DllImport(NativePlugin.LibraryName)]
            [return: OwnReceive]
            public static extern IntPtr wasm_memory_new(
                Store.NativeHandle store,
                [Const] MemoryType.NativeHandle type);

            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_memory_delete(
                [OwnPass] [In] IntPtr handle);

            [DllImport(NativePlugin.LibraryName)]
            [return: OwnReceive]
            public static extern IntPtr wasm_memory_type(
                [Const] NativeHandle handle);

            [DllImport(NativePlugin.LibraryName)]
            public static extern unsafe byte* wasm_memory_data(
                NativeHandle handle);

            [DllImport(NativePlugin.LibraryName)]
            public static extern nuint wasm_memory_data_size(
                [Const] NativeHandle handle);

            [DllImport(NativePlugin.LibraryName)]
            public static extern uint wasm_memory_size(
                [Const] NativeHandle handle);

            [DllImport(NativePlugin.LibraryName)]
            public static extern bool wasm_memory_grow(
                NativeHandle handle,
                uint delta);
        }
    }
}