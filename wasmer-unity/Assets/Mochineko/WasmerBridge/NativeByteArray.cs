using System;
using System.Runtime.InteropServices;

namespace Mochineko.WasmerBridge
{
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct NativeByteArray : IDisposable
    {
        internal nuint size;
        internal byte* data;

        public static NativeByteArray New()
        {
            WasmAPIs.wasm_byte_vec_new_empty(out var array);

            return array;
        }
        
        public static NativeByteArray New(nuint size, byte* data)
        {
            WasmAPIs.wasm_byte_vec_new(out var array, size, data);

            return array;
        }

        public static NativeByteArray CreateFromManaged(byte[] byteArray)
        {
            var copied = Marshal.AllocHGlobal(Marshal.SizeOf<byte>() * byteArray.Length);
            Marshal.Copy(byteArray, 0, copied, byteArray.Length);
            
            var array = New((nuint)byteArray.Length, (byte*)copied);

            Marshal.FreeHGlobal(copied);
            
            return array;
        }

        public void Dispose()
        {
            WasmAPIs.wasm_byte_vec_delete(this);
        }

        private static class WasmAPIs
        {
            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_byte_vec_new_empty(out NativeByteArray vec);
            
            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_byte_vec_new_uninitialized(out NativeByteArray vec, nuint size);
            
            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_byte_vec_new(out NativeByteArray vec, nuint size, byte* data);

            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_byte_vec_copy(out NativeByteArray destination, in NativeByteArray source);
            
            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_byte_vec_delete(in NativeByteArray vec);
        }
    }
}