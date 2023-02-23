using System;
using System.Runtime.InteropServices;
using Mochineko.WasmerBridge.Attributes;

namespace Mochineko.WasmerBridge
{
    [OwnVector]
    [StructLayout(LayoutKind.Sequential)]
    internal readonly unsafe struct ImportTypeVector : IDisposable
    {
        internal readonly nuint size;
        internal readonly IntPtr* data;

        internal static void NewEmpty([OwnOut] out ImportTypeVector vector)
        {
            WasmAPIs.wasm_importtype_vec_new_empty(out vector);
        }

        public static void New([OwnPass] in ReadOnlySpan<ImportType> importTypes, [OwnOut] out ImportTypeVector vector)
        {
            var size = importTypes.Length;
            if (size == 0)
            {
                NewEmpty(out vector);
                return;
            }

            WasmAPIs.wasm_importtype_vec_new_uninitialized(out vector, (nuint)size);

            for (var i = 0; i < size; ++i)
            {
                var importType = importTypes[i];
                vector.data[i] = importType.Handle.DangerousGetHandle();
                // Memory of ImportType is released by Vector then passes ownership to native.
                importType.Handle.SetHandleAsInvalid();
            }
        }

        private static void New(nuint size, [OwnPass] IntPtr* data, [OwnOut] out ImportTypeVector vector)
        {
            WasmAPIs.wasm_importtype_vec_new(out vector, size, data);
        }

        public void Dispose()
        {
            WasmAPIs.wasm_importtype_vec_delete(in this);
        }

        private static class WasmAPIs
        {
            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_importtype_vec_new_empty(
                [OwnOut] out ImportTypeVector vector);

            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_importtype_vec_new_uninitialized(
                [OwnOut] out ImportTypeVector vector,
                nuint size);

            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_importtype_vec_new(
                [OwnOut] out ImportTypeVector vector,
                nuint size,
                [OwnPass] [In] IntPtr* data);

            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_importtype_vec_copy(
                [OwnOut] out ImportTypeVector destination,
                [Const] in ImportTypeVector source);

            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_importtype_vec_delete(
                [OwnPass] in ImportTypeVector vector);
        }
    }
}