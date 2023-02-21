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

        public static void New(out ImportTypeVector vector)
        {
            throw new NotImplementedException();
        }

        internal static void NewEmpty(out ImportTypeVector vector)
        {
            WasmAPIs.wasm_importtype_new_vec_new_empty(out vector);
        }

        private static void New(nuint size, IntPtr* data, out ImportTypeVector vector)
        {
            WasmAPIs.wasm_importtype_new_vec_new(out vector, size, data);
        }

        public void Dispose()
        {
            WasmAPIs.wasm_importtype_vec_delete(in this);
        }

        private static class WasmAPIs
        {
            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_importtype_new_vec_new_empty(
                [OwnOut] [Out] out ImportTypeVector vector);

            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_importtype_new_vec_new_uninitialized(
                [OwnOut] [Out] out ImportTypeVector vector,
                nuint size);

            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_importtype_new_vec_new(
                [OwnOut] [Out] out ImportTypeVector vector,
                nuint size,
                [OwnPass] [In] IntPtr* data);

            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_importtype_vec_copy(
                [OwnOut] [Out] out ImportTypeVector destination,
                [Const] in ImportTypeVector source);

            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_importtype_vec_delete(
                [OwnPass] [In] in ImportTypeVector vector);
        }
    }
}