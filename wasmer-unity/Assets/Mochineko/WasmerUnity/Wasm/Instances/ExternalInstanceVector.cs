using System;
using System.Runtime.InteropServices;
using Mochineko.WasmerUnity.Wasm.Attributes;

namespace Mochineko.WasmerUnity.Wasm.Instances
{
    [OwnVector]
    [StructLayout(LayoutKind.Sequential)]
    internal readonly unsafe struct ExternalInstanceVector : IDisposable
    {
        internal readonly nuint size;
        internal readonly IntPtr* data;

        internal static void New(
            [OwnPass] in ReadOnlySpan<ExternalInstance> instances,
            [OwnOut] out ExternalInstanceVector vector)
        {
            var size = instances.Length;
            if (size == 0)
            {
                NewEmpty(out vector);
                return;
            }

            WasmAPIs.wasm_extern_vec_new_uninitialized(out vector, (nuint)size);
            
            for (var i = 0; i < size; ++i)
            {
                var instance = instances[i];
                vector.data[i] = instance.Handle.DangerousGetHandle();
                // Memory of ExternalInstance is released by Vector then passes ownership to native.
                instance.Handle.SetHandleAsInvalid();
            }
        }

        internal static void NewEmpty([OwnOut] out ExternalInstanceVector instanceVector)
        {
            WasmAPIs.wasm_extern_vec_new_empty(out instanceVector);
        }

        private static void New(nuint size, [OwnPass] IntPtr* data, [OwnOut] out ExternalInstanceVector instanceVector)
        {
            WasmAPIs.wasm_extern_vec_new(out instanceVector, size, data);
        }

        internal void ToManaged(out ReadOnlySpan<ExternalInstance> managed)
        {
            if (size == 0)
            {
                managed = new Span<ExternalInstance>();
                return;
            }

            var array = new ExternalInstance[(int)size];
            for (var i = 0; i < (int)size; i++)
            {
                array[i] = ExternalInstance.FromPointer(data[i], false);
            }

            managed = array;
        }

        public void Dispose()
        {
            WasmAPIs.wasm_extern_vec_delete(in this);
        }

        private static class WasmAPIs
        {
            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_extern_vec_new_empty(
                [OwnOut] out ExternalInstanceVector vector);

            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_extern_vec_new_uninitialized(
                [OwnOut] out ExternalInstanceVector vector,
                nuint size);

            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_extern_vec_new(
                [OwnOut] out ExternalInstanceVector vector,
                nuint size,
                [OwnPass] [In] IntPtr* data);

            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_extern_vec_delete(
                [OwnPass] in ExternalInstanceVector handle);
        }
    }
}