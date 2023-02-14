using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Mochineko.WasmerBridge.Tests
{
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct ValueTypeArray : IDisposable
    {
        internal readonly nuint size;
        internal readonly IntPtr* data;
        
        public ValueTypeArray(IReadOnlyList<ValueKind> kinds)
        {
            WasmAPIs.wasm_valtype_vec_new_uninitialized(out var vec, (nuint)kinds.Count);
            this.size = vec.size;
            this.data = vec.data;

            for (var i = 0; i < kinds.Count; ++i)
            {
                this.data[i] = new ValueType(kinds[i]).Handle.DangerousGetHandle();
            }
        }
        
        // TODO: ToArray

        public void Dispose()
        {
            WasmAPIs.wasm_valtype_vec_delete(this);
        }

        private static class WasmAPIs
        {
            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_valtype_vec_new_empty(out ValueTypeArray vec);
            
            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_valtype_vec_new_uninitialized(out ValueTypeArray vec, nuint size);
            
            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_valtype_vec_new(out ValueTypeArray vec, nuint size, IntPtr* data);

            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_valtype_vec_copy(out ValueTypeArray destination, in ValueTypeArray source);
            
            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_valtype_vec_delete(in ValueTypeArray vec);
        }
    }
}