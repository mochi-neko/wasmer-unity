using System;
using System.Runtime.InteropServices;

namespace Mochineko.WasmerBridge.Tests
{
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct ImportTypeArray : IDisposable
    {
        internal nuint size;
        internal byte* data;

        public static ImportTypeArray New()
        {
            WasmAPIs.wasm_importtype_vec_new_empty(out var array);

            return array;
        }
        
        public static ImportTypeArray New(nuint size, byte* data)
        {
            WasmAPIs.wasm_importtype_vec_new(out var array, size, data);

            return array;
        }

        public void Dispose()
        {
            WasmAPIs.wasm_importtype_vec_delete(this);
        }

        private static class WasmAPIs
        {
            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_importtype_vec_new_empty(out ImportTypeArray vec);
            
            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_importtype_vec_new_uninitialized(out ImportTypeArray vec, nuint size);
            
            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_importtype_vec_new(out ImportTypeArray vec, nuint size, byte* data);

            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_importtype_vec_copy(out ImportTypeArray destination, in ImportTypeArray source);
            
            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_importtype_vec_delete(in ImportTypeArray vec);
        }
    }
}