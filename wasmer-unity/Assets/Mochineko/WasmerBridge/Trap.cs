using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using Mochineko.WasmerBridge.Attributes;

namespace Mochineko.WasmerBridge
{
    [OwnPointed]
    internal sealed class Trap : IDisposable
    {
        internal string Message
        {
            get
            {
                WasmAPIs.wasm_trap_message(Handle, out var vector);
                using (vector)
                {
                    return vector.ToText();
                }
            }
        }

        [return: OwnReceive]
        internal static Trap NewWithEmptyMessage(Store store)
        {
            ByteVector.NewEmpty(out var vector);

            using (vector)
            {
                return New(store, in vector);
            }
        }

        [return: OwnReceive]
        internal static Trap New(Store store, string message)
        {
            ByteVector.FromText(message, out var vector);

            using (vector)
            {
                return New(store, in vector);
            }
        }

        [return: OwnReceive]
        private static Trap New(Store store, in ByteVector message)
        {
            return new Trap(WasmAPIs.wasm_trap_new(store.Handle, in message), hasOwnership: true);
        }

        internal static Trap FromPointer(IntPtr ptr, bool hasOwnership)
            => new Trap(ptr, hasOwnership);

        private Trap(IntPtr handle, bool hasOwnership)
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
                    throw new ObjectDisposedException(typeof(Trap).FullName);
                }

                return handle;
            }
        }

        internal sealed class NativeHandle : SafeHandleMinusOneIsInvalid
        {
            public NativeHandle(IntPtr handle, bool ownsHandle)
                : base(ownsHandle)
            {
                SetHandle(handle);
            }

            protected override bool ReleaseHandle()
            {
                WasmAPIs.wasm_trap_delete(handle);
                return true;
            }
        }

        private static class WasmAPIs
        {
            [DllImport(NativePlugin.LibraryName)]
            [return: OwnReceive]
            public static extern IntPtr wasm_trap_new(
                Store.NativeHandle store,
                [Const] in ByteVector message);

            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_trap_delete(
                [OwnPass] [In] IntPtr handle);

            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_trap_message(
                [Const] NativeHandle trap,
                [OwnOut] out ByteVector message);

            [DllImport(NativePlugin.LibraryName)]
            [return: OwnReceive]
            public static extern IntPtr wasm_trap_origin(
                [Const] NativeHandle trap);

            // TODO:
            // [DllImport(NativePlugin.LibraryName)]
            // public static extern void wasm_trap_trace([Const] in NativeHandle trap, [OwnOut] out FrameVector vector);
        }
    }
}