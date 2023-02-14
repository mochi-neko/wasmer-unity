using System;
using System.Runtime.InteropServices;

namespace Mochineko.WasmerBridge
{
    [StructLayout(LayoutKind.Sequential)]
    internal readonly unsafe struct ValueTypeArray : IDisposable
    {
        internal readonly nuint size;
        internal readonly IntPtr* data;
        
        internal static ValueTypeArray New()
        {
            WasmAPIs.wasm_valtype_vec_new_empty(out var vec);

            return vec;
        }

        internal static ValueTypeArray New(nuint size, ValueType.NativeHandle data)
        {
            WasmAPIs.wasm_valtype_vec_new(out var array, size, data);

            return array;
        }

        internal static ValueTypeArray New(ValueType.NativeHandle[] valueTypes)
        {
            if (valueTypes is null)
            {
                throw new ArgumentNullException(nameof(valueTypes));
            }

            if (valueTypes.Length == 0)
            {
                return New();
            }
            else
            {
                return New((nuint)valueTypes.Length,  valueTypes[0]);
            }
        }

        public void Dispose()
        {
            WasmAPIs.wasm_valtype_vec_delete(this);
        }

        private static class WasmAPIs
        {
            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_valtype_vec_new_empty(out ValueTypeArray vec);
            
            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_valtype_vec_new_uninitialized(out  ValueTypeArray vec, nuint size);
            
            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_valtype_vec_new(out ValueTypeArray vec, nuint size, ValueType.NativeHandle data);

            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_valtype_vec_copy(out ValueTypeArray destination, in ValueTypeArray source);
            
            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_valtype_vec_delete(in ValueTypeArray vec);
        }
    }
}