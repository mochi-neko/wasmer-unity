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
                if (handle.IsInvalid)
                {
                    throw new ObjectDisposedException(typeof(Trap).FullName);
                }

                WasmAPIs.wasm_trap_message(handle, out var vector);

                // ByteVector of message is managed in Trap then We don't need to release ByteVector.
                return vector.ToString();
            }
        }

        internal static Trap New(Store store, string message)
        {
            ByteVector.FromString(message, out var vector);
            
            // ByteVector of message is managed in Trap then We don't need to release ByteVector.
            return New(store, in vector);
        }

        private static Trap New(Store store, in ByteVector message)
        {
            return new Trap(WasmAPIs.wasm_trap_new(store.Handle, in message));
        }

        private Trap(IntPtr handle)
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
                    throw new ObjectDisposedException(typeof(Trap).FullName);
                }

                return handle;
            }
        }

        internal sealed class NativeHandle : SafeHandleMinusOneIsInvalid
        {
            public NativeHandle(IntPtr handle)
                : base(true)
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
                [Const] [OwnPass]
                in ByteVector message); // NOTE: Does not attributed "own" in C++ but message also is deleted when trap is deleted. 

            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_trap_delete([OwnPass] IntPtr trap);

            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_trap_message([Const] NativeHandle trap, [OwnOut] out ByteVector message);

            [DllImport(NativePlugin.LibraryName)]
            [return: OwnReceive]
            public static extern IntPtr wasm_trap_origin([Const] NativeHandle trap);

            // TODO:
            // [DllImport(NativePlugin.LibraryName)]
            // public static extern void wasm_trap_trace([Const] in NativeHandle trap, [OwnOut] out FrameVector vector);
        }
    }
}