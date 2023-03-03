using System;
using System.Runtime.InteropServices;
using Mochineko.WasmerUnity.Wasm.Attributes;

namespace Mochineko.WasmerUnity.Wasm.Types
{
    [OwnVector]
    [StructLayout(LayoutKind.Sequential)]
    internal readonly unsafe struct ValueTypeVector : IDisposable
    {
        internal readonly nuint size;
        internal readonly IntPtr* data;

        public static void New(in ReadOnlySpan<ValueKind> kinds, [OwnOut] out ValueTypeVector vector)
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
                var valueType = ValueType.FromKind(kinds[i]);
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
                array[i] = ValueType.KindFromPointer(data[i]);
            }

            kinds = array;
        }

        internal static void NewEmpty([OwnOut] out ValueTypeVector vector)
        {
            WasmAPIs.wasm_valtype_vec_new_empty(out vector);
        }

        private static void New(nuint size, [OwnPass] IntPtr* data, [OwnOut] out ValueTypeVector vector)
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
                [OwnOut] out ValueTypeVector vector);

            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_valtype_vec_new_uninitialized(
                [OwnOut] out ValueTypeVector vector,
                nuint size);

            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_valtype_vec_new(
                [OwnOut] out ValueTypeVector vector,
                nuint size,
                [OwnPass] [In] IntPtr* data);

            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_valtype_vec_delete(
                [OwnPass] in ValueTypeVector handle);
        }
    }
}