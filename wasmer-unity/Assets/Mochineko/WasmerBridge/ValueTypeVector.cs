using System;
using System.Runtime.InteropServices;
using Mochineko.WasmerBridge.Attributes;

namespace Mochineko.WasmerBridge
{
    [OwnVector]
    [StructLayout(LayoutKind.Sequential)]
    internal readonly unsafe struct ValueTypeVector : IDisposable
    {
        internal readonly nuint size;
        internal readonly IntPtr* data;

        internal static ValueTypeVector NewEmpty()
        {
            WasmAPIs.wasm_valtype_vec_new_empty(out var vector);

            return vector;
        }

        private static ValueTypeVector New(nuint size, IntPtr* data)
        {
            WasmAPIs.wasm_valtype_vec_new(out var vector, size, data);

            return vector;
        }

        public static ValueTypeVector New(ReadOnlySpan<ValueKind> valueKinds)
        {
            var size = valueKinds.Length;
            if (size == 0)
            {
                return NewEmpty();
            }

            WasmAPIs.wasm_valtype_vec_new_uninitialized(out var vector, (nuint)size);

            for (var i = 0; i < size; ++i)
            {
                vector.data[i] = ValueType.New(valueKinds[i]).Handle.DangerousGetHandle();
            }

            return vector;
        }

        public static void ToValueKinds(in ValueTypeVector valueTypes, out ReadOnlySpan<ValueKind> valueKinds)
        {
            if (valueTypes.size == 0)
            {
                valueKinds = new Span<ValueKind>();
            }
            else
            {
                valueKinds = new Span<ValueKind>(valueTypes.data, (int)valueTypes.size);
            }
        }

        public void Dispose()
        {
            WasmAPIs.wasm_valtype_vec_delete(this);
        }

        private static class WasmAPIs
        {
            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_valtype_vec_new_empty([OwnOut] out ValueTypeVector vector);

            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_valtype_vec_new_uninitialized([OwnOut] out ValueTypeVector vector,
                nuint size);

            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_valtype_vec_new([OwnOut] out ValueTypeVector vector, nuint size,
                [OwnPass] IntPtr* data);

            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_valtype_vec_copy([OwnOut] out ValueTypeVector destination,
                [Const] in ValueTypeVector source);

            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_valtype_vec_delete([OwnPass] ValueTypeVector vector);
        }
    }
}