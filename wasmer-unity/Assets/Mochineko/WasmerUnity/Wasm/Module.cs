using System;
using System.Runtime.InteropServices;
using Mochineko.WasmerUnity.Wasm.Attributes;
using Microsoft.Win32.SafeHandles;

namespace Mochineko.WasmerUnity.Wasm
{
    [OwnPointed]
    public sealed class Module : IDisposable
    {
        internal void Imports([OwnOut] out ImportTypeVector vector)
        {
            WasmAPIs.wasm_module_imports(Handle, out vector);
        }

        internal void Exports([OwnOut] out ExportTypeVector vector)
        {
            WasmAPIs.wasm_module_exports(Handle, out vector);
        }

        public static bool Validate(Store store, in ReadOnlySpan<byte> binary)
        {
            if (store is null)
            {
                throw new ArgumentNullException(nameof(store));
            }

            if (binary.Length == 0)
            {
                throw new ArgumentNullException(nameof(binary));
            }

            ByteVector.New(in binary, out var vector);
            using (vector)
            {
                return WasmAPIs.wasm_module_validate(store.Handle, in vector);
            }
        }

        [return: OwnReceive]
        public static Module New(Store store, in ReadOnlySpan<byte> wasm)
        {
            ByteVector.New(in wasm, out var vector);
            using (vector)
            {
                return New(store, in vector);
            }
        }

        [return: OwnReceive]
        internal static Module New(Store store, in ByteVector binary)
        {
            if (store is null)
            {
                throw new ArgumentNullException(nameof(store));
            }

            if (binary.size == 0)
            {
                throw new ArgumentNullException(nameof(binary));
            }

            var handle = WasmAPIs.wasm_module_new(store.Handle, in binary);
            if (handle == IntPtr.Zero)
            {
                throw new InvalidOperationException("Failed to create module.");
            }

            return new Module(handle, hasOwnership: true);
        }

        public void Serialize(out ReadOnlySpan<byte> serialized)
        {
            SerializeNative(out var binary);

            using (binary)
            {
                unsafe
                {
                    // Copy from native to C#
                    var array = new byte[binary.size];
                    Marshal.Copy((IntPtr)binary.data, array, 0, (int)binary.size);
                    serialized = array;
                }
            }
        }

        private void SerializeNative([OwnOut] out ByteVector binary)
        {
            WasmAPIs.wasm_module_serialize(Handle, out binary);
        }

        [return: OwnReceive]
        public static Module Deserialize(Store store, in ReadOnlySpan<byte> binary)
        {
            if (store is null)
            {
                throw new ArgumentNullException(nameof(store));
            }

            if (binary.Length == 0)
            {
                throw new ArgumentNullException(nameof(binary));
            }

            ByteVector.New(in binary, out var vector);
            using (vector)
            {
                return DeserializeNative(store, vector);
            }
        }

        [return: OwnReceive]
        private static Module DeserializeNative(Store store, in ByteVector binary)
        {
            if (store is null)
            {
                throw new ArgumentNullException(nameof(store));
            }

            if (binary.size == 0)
            {
                throw new ArgumentNullException(nameof(binary));
            }

            return new Module(
                WasmAPIs.wasm_module_deserialize(store.Handle, in binary),
                hasOwnership: true);
        }

        private Module(IntPtr handle, bool hasOwnership)
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
                    throw new ObjectDisposedException(typeof(Module).FullName);
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
                WasmAPIs.wasm_module_delete(handle);
                return true;
            }
        }

        private static class WasmAPIs
        {
            [DllImport(NativePlugin.LibraryName)]
            [return: OwnReceive]
            public static extern IntPtr wasm_module_new(
                Store.NativeHandle store,
                [ConstVector] in ByteVector binary);

            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_module_delete(
                [OwnPass] [In] IntPtr handle);

            [DllImport(NativePlugin.LibraryName)]
            public static extern bool wasm_module_same(
                [Const] NativeHandle left,
                [Const] NativeHandle right);

            [DllImport(NativePlugin.LibraryName)]
            public static extern bool wasm_module_validate(
                Store.NativeHandle store,
                [ConstVector] in ByteVector binary);

            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_module_imports(
                [Const] NativeHandle handle,
                [OwnOut] out ImportTypeVector imports);

            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_module_exports(
                [Const] NativeHandle handle,
                [OwnOut] out ExportTypeVector exports);

            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_module_serialize(
                [Const] NativeHandle handle,
                [OwnOut] out ByteVector binary);

            [DllImport(NativePlugin.LibraryName)]
            [return: OwnReceive]
            public static extern IntPtr wasm_module_deserialize(
                Store.NativeHandle store,
                [ConstVector] in ByteVector binary);
        }
    }
}