using System;
using System.Runtime.InteropServices;
using Mochineko.WasmerBridge.OwnAttributes;

namespace Mochineko.WasmerBridge
{
    [OwnVector]
    [StructLayout(LayoutKind.Sequential)]
    internal readonly unsafe struct ByteVector : IDisposable
    {
        internal readonly nuint size;
        internal readonly byte* data;

        internal static ByteVector New()
        {
            WasmAPIs.wasm_byte_vec_new_empty(out var vector);

            return vector;
        }
        
        private static ByteVector New(nuint size, byte* data)
        {
            WasmAPIs.wasm_byte_vec_new(out var vector, size, data);

            return vector;
        }

        public static ByteVector New(in ReadOnlySpan<byte> binary)
        {
            Span<byte> copy = stackalloc byte[binary.Length];
            binary.CopyTo(copy);
            fixed (byte* data = copy)
            {
                return New((nuint)binary.Length, data);
            }
        }

        internal static void ToManagedSpan(in ByteVector vector, out ReadOnlySpan<byte> binary)
        {
            binary = new Span<byte>(vector.data, (int)vector.size);
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
            public static extern void wasm_byte_vec_new([OwnOut]out ByteVector vector, nuint size, [OwnParameter]byte* data);

            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_byte_vec_copy([OwnOut]out ByteVector destination, [ConstVector]in ByteVector source);
            
            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_byte_vec_delete([OwnParameter]in ByteVector vector);
        }
    }
}