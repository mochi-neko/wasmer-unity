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

        internal static void New(
            in ReadOnlySpan<ExternalKind> kinds,
            [OwnOut] out ExternalInstanceVector instanceVector)
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

        internal static void NewEmpty([OwnOut] out ExternalInstanceVector instanceVector)
        {
            WasmAPIs.wasm_extern_vec_new_empty(out instanceVector);
        }

        private static void New(nuint size, [OwnPass] IntPtr* data, [OwnOut]  out ExternalInstanceVector instanceVector)
        {
            WasmAPIs.wasm_extern_vec_new(out instanceVector, size, data);
        }

        public void ToKinds(out ReadOnlySpan<ExternalKind> kinds)
        {
            // TODO:
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            WasmAPIs.wasm_extern_vec_delete(in this);
        }

        private static class WasmAPIs
        {
            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_extern_vec_new_empty(
                [OwnOut] out ExternalInstanceVector instanceVector);

            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_extern_vec_new_uninitialized(
                [OwnOut] out ExternalInstanceVector instanceVector,
                nuint size);

            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_extern_vec_new(
                [OwnOut] out ExternalInstanceVector instanceVector,
                nuint size,
                [OwnPass] [In] IntPtr* data);

            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_extern_vec_copy(
                [OwnOut] out ExternalInstanceVector destination,
                [Const] in ExternalInstanceVector source);

            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_extern_vec_delete(
                [OwnPass] in ExternalInstanceVector instanceVector);
        }
    }
}