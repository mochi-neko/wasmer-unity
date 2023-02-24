using System;
using System.Runtime.InteropServices;
using Mochineko.WasmerBridge.Attributes;

namespace Mochineko.WasmerBridge
{
    [OwnVector]
    [StructLayout(LayoutKind.Sequential)]
    internal readonly unsafe struct ValueInstanceVector : IDisposable
    {
        internal readonly nuint size;
        internal readonly ValueInstance* data;

        internal static void NewEmpty([OwnOut] out ValueInstanceVector vector)
        {
            WasmAPIs.wasm_val_vec_new_empty(out vector);
        }

        private static void New(nuint size, [OwnPass] ValueInstance* data, [OwnOut] out ValueInstanceVector vector)
        {
            WasmAPIs.wasm_val_vec_new(out vector, size, data);
        }

        internal static void New(in ReadOnlySpan<ValueInstance> array, [OwnOut] out ValueInstanceVector vector)
        {
            Span<ValueInstance> copy = stackalloc ValueInstance[array.Length];
            array.CopyTo(copy);
            fixed (ValueInstance* data = copy)
            {
                New((nuint)array.Length, data, out vector);
            }
        }

        internal void ToManaged(out ReadOnlySpan<ValueInstance> binary)
        {
            var span = new Span<ValueInstance>(data, (int)size);
            var copied = new Span<ValueInstance>(new ValueInstance[(int)size]);
            span.CopyTo(copied);

            binary = copied;
        }

        public void Dispose()
        {
            WasmAPIs.wasm_val_vec_delete(in this);
        }

        private static class WasmAPIs
        {
            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_val_vec_new_empty(
                [OwnOut] out ValueInstanceVector vector);

            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_val_vec_new_uninitialized(
                [OwnOut] out ValueInstanceVector vector,
                nuint size);

            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_val_vec_new(
                [OwnOut] out ValueInstanceVector vector,
                nuint size,
                [OwnPass] [In] ValueInstance* data);

            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_val_vec_delete(
                [OwnPass] in ValueInstanceVector handle);
        }
    }
}