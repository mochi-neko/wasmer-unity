using System;
using System.Runtime.InteropServices;
using Mochineko.WasmerBridge.Attributes;

namespace Mochineko.WasmerBridge
{
    [OwnVector]
    [StructLayout(LayoutKind.Sequential)]
    internal readonly unsafe struct ExternalVector : IDisposable
    {
        internal readonly nuint size;
        internal readonly IntPtr* data;

        internal static void New(in ReadOnlySpan<ExternalKind> kinds, out ExternalVector vector)
        {
            var size = kinds.Length;
            if (size == 0)
            {
                NewEmpty(out vector);
                return;
            }

            WasmAPIs.wasm_extern_vec_new_uninitialized(out vector, (nuint)size);

            // TODO:
            for (var i = 0; i < size; ++i)
            {
                //vector.data[i] = ValueType.New(kinds[i]).Handle.DangerousGetHandle();
            }
        }
        
        internal static void NewEmpty(out ExternalVector vector)
        {
            WasmAPIs.wasm_extern_vec_new_empty(out vector);
        }

        private static void New(nuint size, IntPtr* data, out ExternalVector vector)
        {
            WasmAPIs.wasm_extern_vec_new(out vector, size, data);
        }
        
        public void ToKinds(out ReadOnlySpan<ExternalKind> kinds)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            WasmAPIs.wasm_extern_vec_delete(this);
        }

        private static class WasmAPIs
        {
            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_extern_vec_new_empty([OwnOut] out ExternalVector vector);

            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_extern_vec_new_uninitialized(
                [OwnOut] out ExternalVector vector,
                nuint size);

            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_extern_vec_new(
                [OwnOut] out ExternalVector vector,
                nuint size,
                [OwnPass] IntPtr* data);

            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_extern_vec_copy(
                [OwnOut] out ExternalVector destination,
                [Const] in ExternalVector source);

            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_extern_vec_delete([OwnPass] in ExternalVector vector);
        }
    }
}