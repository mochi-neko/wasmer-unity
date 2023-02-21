using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using Mochineko.WasmerBridge.Attributes;

namespace Mochineko.WasmerBridge
{
    [OwnPointed]
    public sealed class Module : IDisposable
    {
        internal void Imports (out ImportTypeVector vector)
        {
            WasmAPIs.wasm_module_imports(Handle, out vector);
        }
        
        internal void Exports (out ExportTypeVector vector)
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

        public static Module NewFromBinary(Store store, in ReadOnlySpan<byte> wasm)
        {
            ByteVector.New(in wasm, out var vector);
            using (vector)
            {
                return New(store, in vector);
            }
        }

        public static Module NewFromWat(Store store, string wat)
        {
            wat.FromWatToWasm(out var wasm);
            using (wasm)
            {
                return New(store, wasm);
            }
        }

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

            return new Module(handle);
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

        private void SerializeNative(out ByteVector binary)
        {
            WasmAPIs.wasm_module_serialize(Handle, out binary);
        }

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

            return new Module(WasmAPIs.wasm_module_deserialize(store.Handle, in binary));
        }

        private Module(IntPtr handle)
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
                    throw new ObjectDisposedException(typeof(Module).FullName);
                }

                return handle;
            }
        }

        internal sealed class NativeHandle : SafeHandleZeroOrMinusOneIsInvalid
        {
            public NativeHandle(IntPtr handle)
                : base(true)
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
                [OwnPass] [In] IntPtr module);

            [DllImport(NativePlugin.LibraryName)]
            [return: OwnReceive]
            public static extern IntPtr wasm_module_copy(
                [Const] NativeHandle module);

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
                [Const] NativeHandle module,
                [OwnOut] [Out] out ImportTypeVector importTypes);

            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_module_exports(
                [Const] NativeHandle module,
                [OwnOut][Out] out ExportTypeVector exportTypes);

            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_module_serialize(
                [Const] NativeHandle module,
                [OwnOut] [Out] out ByteVector binary);

            [DllImport(NativePlugin.LibraryName)]
            [return: OwnReceive]
            public static extern IntPtr wasm_module_deserialize(
                Store.NativeHandle store,
                [ConstVector] in ByteVector binary);
        }
    }
}