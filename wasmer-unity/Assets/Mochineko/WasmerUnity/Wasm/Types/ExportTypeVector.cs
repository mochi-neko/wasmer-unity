using System;
using System.Runtime.InteropServices;
using Mochineko.WasmerUnity.Wasm.Attributes;

namespace Mochineko.WasmerUnity.Wasm.Types
{
    [OwnVector]
    [StructLayout(LayoutKind.Sequential)]
    internal readonly unsafe struct ExportTypeVector : IDisposable
    {
        internal readonly nuint size;
        internal readonly IntPtr* data;

        internal static void NewEmpty([OwnOut] out ExportTypeVector vector)
        {
            WasmAPIs.wasm_exporttype_vec_new_empty(out vector);
        }

        public static void New([OwnPass] in ReadOnlySpan<ExportType> exportTypes, [OwnOut] out ExportTypeVector vector)
        {
            var size = exportTypes.Length;
            if (size == 0)
            {
                NewEmpty(out vector);
                return;
            }

            WasmAPIs.wasm_exporttype_vec_new_uninitialized(out vector, (nuint)size);

            for (var i = 0; i < size; ++i)
            {
                var exportType = exportTypes[i];
                vector.data[i] = exportType.Handle.DangerousGetHandle();
                // Memory of ExportType is released by Vector then passes ownership to native.
                exportType.Handle.SetHandleAsInvalid();
            }
        }

        private static void New(nuint size, [OwnPass] IntPtr* data, [OwnOut] out ExportTypeVector vector)
        {
            WasmAPIs.wasm_exporttype_vec_new(out vector, size, data);
        }

        internal void ToManaged(out ReadOnlySpan<ExportType> managed)
        {
            if (size == 0)
            {
                managed = new Span<ExportType>();
                return;
            }

            var array = new ExportType[(int)size];
            for (var i = 0; i < (int)size; i++)
            {
                array[i] = ExportType.FromPointer(data[i], false);
            }

            managed = array;
        }

        public void Dispose()
        {
            WasmAPIs.wasm_exporttype_vec_delete(in this);
        }

        private static class WasmAPIs
        {
            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_exporttype_vec_new_empty(
                [OwnOut] out ExportTypeVector vector);

            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_exporttype_vec_new_uninitialized(
                [OwnOut] out ExportTypeVector vector,
                nuint size);

            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_exporttype_vec_new(
                [OwnOut] out ExportTypeVector vector,
                nuint size,
                [OwnPass] [In] IntPtr* data);

            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_exporttype_vec_delete(
                [OwnPass] in ExportTypeVector handle);
        }
    }
}