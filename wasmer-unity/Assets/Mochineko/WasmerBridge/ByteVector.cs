using System;
using System.Runtime.InteropServices;
using System.Text;
using Mochineko.WasmerBridge.Attributes;

namespace Mochineko.WasmerBridge
{
    [OwnVector]
    [StructLayout(LayoutKind.Sequential)]
    internal readonly unsafe struct ByteVector : IDisposable
    {
        internal readonly nuint size;
        internal readonly byte* data;

        internal static void NewEmpty(out ByteVector vector)
        {
            WasmAPIs.wasm_byte_vec_new_empty(out vector);
        }

        private static void New(nuint size, byte* data, out ByteVector vector)
        {
            WasmAPIs.wasm_byte_vec_new(out vector, size, data);
        }

        // Avoid copy of struct by using "out"
        internal static void New(in ReadOnlySpan<byte> binary, out ByteVector vector)
        {
            Span<byte> copy = stackalloc byte[binary.Length];
            binary.CopyTo(copy);
            fixed (byte* data = copy)
            {
                New((nuint)binary.Length, data, out vector);
            }
        }

        internal void ToManaged(out ReadOnlySpan<byte> binary)
        {
            var span = new Span<byte>(data, (int)size);
            var copied = new Span<byte>(new byte[(int)size]);
            span.CopyTo(copied);

            binary = copied;
        }

        internal static void FromString(string text, out ByteVector vector)
        {
            if (string.IsNullOrEmpty(text))
            {
                NewEmpty(out vector);
                return;
            }

            var textBytes = Encoding.UTF8.GetBytes(text);

            New(textBytes, out vector);
        }

        internal new string ToString()
        {
            if (size == 0)
            {
                return string.Empty;
            }

            this.ToManaged(out var binary);

            // Remove bytes after null
            var indexOfNull = binary.LastIndexOf((byte)0);
            if (indexOfNull != -1)
            {
                binary = binary[..indexOfNull];
            }

            return Encoding.UTF8.GetString(binary);
        }

        public void Dispose()
        {
            WasmAPIs.wasm_byte_vec_delete(in this);
        }

        private static class WasmAPIs
        {
            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_byte_vec_new_empty([OwnOut] out ByteVector vector);

            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_byte_vec_new_uninitialized(
                [OwnOut] out ByteVector vector,
                nuint size);

            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_byte_vec_new(
                [OwnOut] out ByteVector vector,
                nuint size,
                [OwnPass] byte* data);
            
            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_byte_vec_delete([OwnPass] in ByteVector vector);

            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_byte_vec_copy(
                [OwnOut] out ByteVector destination,
                [ConstVector] in ByteVector source);
        }
    }
}