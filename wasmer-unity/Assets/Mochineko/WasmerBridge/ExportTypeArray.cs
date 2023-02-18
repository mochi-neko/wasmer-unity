using System;
using System.Runtime.InteropServices;

namespace Mochineko.WasmerBridge
{
    [StructLayout(LayoutKind.Sequential)]
    internal readonly unsafe struct ExportTypeArray : IDisposable
    {
        internal readonly nuint size;
        internal readonly byte* data;

        public static ExportTypeArray New()
        {
            WasmAPIs.wasm_exporttype_vec_new_empty(out var array);

            return array;
        }
        
        public static ExportTypeArray New(nuint size, byte* data)
        {
            WasmAPIs.wasm_exporttype_vec_new(out var array, size, data);

            return array;
        }

        public void Dispose()
        {
            WasmAPIs.wasm_exporttype_vec_delete(this);
        }

        private static class WasmAPIs
        {
            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_exporttype_vec_new_empty(out ExportTypeArray vec);
            
            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_exporttype_vec_new_uninitialized(out ExportTypeArray vec, nuint size);
            
            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_exporttype_vec_new(out ExportTypeArray vec, nuint size, byte* data);

            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_exporttype_vec_copy(out ExportTypeArray destination, in ExportTypeArray source);
            
            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_exporttype_vec_delete(in ExportTypeArray vec);
        }
    }
}