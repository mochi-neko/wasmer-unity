using System;
using System.Runtime.InteropServices;
using Mochineko.WasmerBridge.Attributes;

namespace Mochineko.WasmerBridge
{
    [OwnVector]
    [StructLayout(LayoutKind.Sequential)]
    internal readonly unsafe struct ExternalInstanceVector : IDisposable
    {
        internal readonly nuint size;
        internal readonly IntPtr* data;

        internal static void New(in ReadOnlySpan<ExternalKind> kinds, out ExternalInstanceVector instanceVector)
        {
            var size = kinds.Length;
            if (size == 0)
            {
                NewEmpty(out instanceVector);
                return;
            }

            WasmAPIs.wasm_extern_vec_new_uninitialized(out instanceVector, (nuint)size);

            // TODO:
            for (var i = 0; i < size; ++i)
            {
                //vector.data[i] = ValueType.New(kinds[i]).Handle.DangerousGetHandle();
            }
        }

        internal static void NewEmpty(out ExternalInstanceVector instanceVector)
        {
            WasmAPIs.wasm_extern_vec_new_empty(out instanceVector);
        }

        private static void New(nuint size, IntPtr* data, out ExternalInstanceVector instanceVector)
        {
            WasmAPIs.wasm_extern_vec_new(out instanceVector, size, data);
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
            public static extern void wasm_extern_vec_new_empty(
                [OwnOut] [Out] out ExternalInstanceVector instanceVector);

            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_extern_vec_new_uninitialized(
                [OwnOut] [Out] out ExternalInstanceVector instanceVector,
                nuint size);

            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_extern_vec_new(
                [OwnOut] [Out] out ExternalInstanceVector instanceVector,
                nuint size,
                [OwnPass] [In] IntPtr* data);

            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_extern_vec_copy(
                [OwnOut] [Out] out ExternalInstanceVector destination,
                [Const] in ExternalInstanceVector source);

            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_extern_vec_delete(
                [OwnPass] [In] in ExternalInstanceVector instanceVector);
        }
    }
}