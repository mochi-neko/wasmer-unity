using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using Mochineko.WasmerBridge.Attributes;

namespace Mochineko.WasmerBridge
{
    [OwnPointed]
    public sealed class Module : IDisposable
    {
        public string Name { get; }

        // private readonly ImportType[] importTypes;
        // private readonly ExportType[] exportTypes;

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

        public static Module NewFromBinary(Store store, string name, in ReadOnlySpan<byte> wasm)
        {
            ByteVector.New(in wasm, out var vector);
            using (vector)
            {
                return New(store, name, in vector);
            }
        }
        
        public static Module NewFromWat(Store store, string name, string wat)
        {
            wat.FromWatToWasm(out var wasm);
            using (wasm)
            {
                return New(store, name, wasm);
            }
        }

        internal static Module New(Store store, string name, in ByteVector binary)
        {
            if (store is null)
            {
                throw new ArgumentNullException(nameof(store));
            }

            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
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

            var module = new Module(handle, name);

            // WasmAPIs.wasm_module_imports(handle, out var imports);
            // using (var _ = imports)
            // {
            //     this.importTypes = imports.ToImportArray();
            // }
            //
            // WasmAPIs.wasm_module_exports(handle, out var exports);
            //
            // using (var _ = exports)
            // {
            //     this.exportTypes = exports.ToExportArray();
            // }

            return module;
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

        internal void SerializeNative(out ByteVector binary)
        {
            WasmAPIs.wasm_module_serialize(handle, out binary);
        }

        public static Module Deserialize(Store store, string name, in ReadOnlySpan<byte> binary)
        {
            ByteVector.New(in binary, out var vector);
            using (vector)
            {
                return DeserializeNative(store, name, vector);
            }
        }

        internal static Module DeserializeNative(Store store, string name, in ByteVector binary)
        {
            if (store is null)
            {
                throw new ArgumentNullException(nameof(store));
            }

            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            return new Module(
                WasmAPIs.wasm_module_deserialize(store.Handle, in binary),
                name);
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

        private Module(IntPtr handle, string name)
        {
            this.handle = new NativeHandle(handle);
            this.Name = name;
        }

        public void Dispose()
        {
            handle.Dispose();
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
            public static extern bool wasm_module_validate(Store.NativeHandle store, [ConstVector] in ByteVector binary);

            [DllImport(NativePlugin.LibraryName)]
            [return: OwnResult]
            public static extern IntPtr
                wasm_module_new(Store.NativeHandle store, [ConstVector] in ByteVector binary);

            // TODO:
            //[DllImport(NativePlugin.LibraryName)]
            //public static extern void wasm_module_imports(NativeHandle module, out ImportTypeArray importTypes);

            // TODO:
            //[DllImport(NativePlugin.LibraryName)]
            //public static extern void wasm_module_exports(NativeHandle module, out ExportTypeArray exportTypes);

            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_module_serialize([Const] NativeHandle module, [OwnOut] out ByteVector binary);
            
            [DllImport(NativePlugin.LibraryName)]
            [return: OwnResult]
            public static extern IntPtr wasm_module_deserialize(Store.NativeHandle store, [ConstVector] in ByteVector binary);

            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_module_delete([OwnParameter] IntPtr module);
        }
    }
}