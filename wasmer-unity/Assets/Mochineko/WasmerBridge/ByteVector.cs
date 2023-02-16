using System;
using System.Runtime.InteropServices;

namespace Mochineko.WasmerBridge
{
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
            fixed (byte* data = binary)
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
            public static extern void wasm_byte_vec_new_empty(out ByteVector vector);
            
            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_byte_vec_new_uninitialized(out ByteVector vector, nuint size);
            
            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_byte_vec_new(out ByteVector vector, nuint size, byte* data);

            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_byte_vec_copy(out ByteVector destination, in ByteVector source);
            
            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_byte_vec_delete(in ByteVector vector);
        }
    }
}