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
            WasmAPIs.wasm_byte_vec_new_empty(out var array);

            return array;
        }
        
        internal static ByteVector New(nuint size, byte* data)
        {
            WasmAPIs.wasm_byte_vec_new(out var array, size, data);

            return array;
        }

        public static ByteVector New(byte[] byteArray)
        {
            // Copy bytes to block memory
            var copied = Marshal.AllocCoTaskMem(Marshal.SizeOf<byte>() * byteArray.Length);
            Marshal.Copy(byteArray, 0, copied, byteArray.Length);
            
            var array = New((nuint)byteArray.Length, (byte*)copied);

            Marshal.FreeCoTaskMem(copied);
            
            return array;
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