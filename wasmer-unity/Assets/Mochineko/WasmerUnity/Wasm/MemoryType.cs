using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using Mochineko.WasmerUnity.Wasm.Attributes;

namespace Mochineko.WasmerUnity.Wasm
{
    [OwnPointed]
    public sealed class MemoryType : IDisposable
    {
        internal Limits Limits
        {
            get
            {
                unsafe
                {
                    return *WasmAPIs.wasm_memorytype_limits(Handle);
                }
            }
        }

        [return: OwnReceive]
        internal static MemoryType New(in Limits limits)
        {
            var handle = WasmAPIs.wasm_memorytype_new(in limits);

            return new MemoryType(handle, hasOwnership: true);
        }
        
        internal static MemoryType FromPointer(IntPtr ptr, bool hasOwnership)
            => new MemoryType(ptr, hasOwnership);

        private MemoryType(IntPtr handle, bool hasOwnership)
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
                    throw new ObjectDisposedException(typeof(MemoryType).FullName);
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
                WasmAPIs.wasm_memorytype_delete(handle);
                return true;
            }
        }

        private static class WasmAPIs
        {
            [DllImport(NativePlugin.LibraryName)]
            [return: OwnReceive]
            public static extern IntPtr wasm_memorytype_new(
                [Const] in Limits limits);

            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_memorytype_delete(
                [OwnPass] [In] IntPtr handle);

            [DllImport(NativePlugin.LibraryName)]
            [return: Const]
            public static extern unsafe Limits* wasm_memorytype_limits(
                [Const] NativeHandle handle);
        }
    }
}