using System;
using System.Runtime.InteropServices;
using Mochineko.WasmerBridge.Attributes;

namespace Mochineko.WasmerBridge
{
    [OwnVector]
    [StructLayout(LayoutKind.Sequential)]
    internal readonly unsafe struct ByteVector : IDisposable
    {
        internal readonly nuint size;
        internal readonly byte* data;

        internal static void NewEmpty(out ByteVector vector)
        {
            WasmAPIs.wasm_byte_vec_new_empty(out vector);
        }
        
        private static void New(nuint size, byte* data, out ByteVector vector)
        {
            WasmAPIs.wasm_byte_vec_new(out vector, size, data);
        }

        // Avoid copy of struct by using "out"
        public static void New(in ReadOnlySpan<byte> binary, out ByteVector vector)
        {
            Span<byte> copy = stackalloc byte[binary.Length];
            binary.CopyTo(copy);
            fixed (byte* data = copy)
            {
                New((nuint)binary.Length, data, out vector);
            }
        }

        internal void ToManagedSpan(out ReadOnlySpan<byte> binary)
        {
            binary = new Span<byte>(data, (int)size);
        }

        public void Dispose()
        {
            WasmAPIs.wasm_byte_vec_delete(this);
        }

        private static class WasmAPIs
        {
            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_byte_vec_new_empty([OwnOut]out ByteVector vector);
            
            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_byte_vec_new_uninitialized([OwnOut]out ByteVector vector, nuint size);
            
            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_byte_vec_new([OwnOut]out ByteVector vector, nuint size, [OwnPass]byte* data);

            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_byte_vec_copy([OwnOut]out ByteVector destination, [ConstVector]in ByteVector source);
            
            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_byte_vec_delete([OwnPass]in ByteVector vector);
        }
    }
}