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

        public static void New(in ReadOnlySpan<ValueKind> kinds, out ValueTypeVector vector)
        {
            var size = kinds.Length;
            if (size == 0)
            {
                NewEmpty(out vector);
                return;
            }

            WasmAPIs.wasm_valtype_vec_new_uninitialized(out vector, (nuint)size);

            for (var i = 0; i < size; ++i)
            {
                var valueType = ValueType.New(kinds[i]);
                vector.data[i] = valueType.Handle.DangerousGetHandle();
                // Memory of ValueType is released by Vector then passes ownership to native.
                valueType.Handle.SetHandleAsInvalid();
            }
        }

        public void ToKinds(out ReadOnlySpan<ValueKind> kinds)
        {
            if (size == 0)
            {
                kinds = new Span<ValueKind>();
                return;
            }

            var array = new ValueKind[(int)size];
            for (var i = 0; i < (int)size; i++)
            {
                array[i] = ValueType.KindFromPtr(data[i]);
            }

            kinds = array;
        }

        internal static void NewEmpty(out ValueTypeVector vector)
        {
            WasmAPIs.wasm_valtype_vec_new_empty(out vector);
        }

        private static void New(nuint size, IntPtr* data, out ValueTypeVector vector)
        {
            WasmAPIs.wasm_valtype_vec_new(out vector, size, data);
        }

        public void Dispose()
        {
            WasmAPIs.wasm_valtype_vec_delete(in this);
        }

        private static class WasmAPIs
        {
            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_valtype_vec_new_empty(
                [OwnOut] [Out] out ValueTypeVector vector);

            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_valtype_vec_new_uninitialized(
                [OwnOut] [Out] out ValueTypeVector vector,
                nuint size);

            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_valtype_vec_new(
                [OwnOut] [Out] out ValueTypeVector vector,
                nuint size,
                [OwnPass] [In] IntPtr* data);

            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_valtype_vec_delete(
                [OwnPass] [In] in ValueTypeVector vector);

            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_valtype_vec_copy(
                [OwnOut] [Out] out ValueTypeVector destination,
                [Const] in ValueTypeVector source);
        }
    }
}