using System;
using System.Runtime.InteropServices;
using Mochineko.WasmerBridge.Attributes;

namespace Mochineko.WasmerBridge
{
    [OwnStruct]
    [StructLayout(LayoutKind.Explicit)]
    internal readonly struct ValueInstance : IDisposable
    {
        internal readonly byte kind;
        internal readonly ValueUnion of;

        [StructLayout(LayoutKind.Explicit)]
        internal readonly unsafe struct ValueUnion
        {
            [FieldOffset(0)] public readonly int i32;
            [FieldOffset(0)] public readonly long i64;
            [FieldOffset(0)] public readonly float f32;
            [FieldOffset(0)] public readonly double f64;
            [FieldOffset(0)] public readonly Reference* reference;
        }

        public void Dispose()
        {
            WasmAPIs.wasm_val_delete(in this);
        }

        private static class WasmAPIs
        {
            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_val_delete(
                [OwnPass] in ValueInstance value);

            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_val_copy(
                [OwnOut] [Out] out ValueInstance copy,
                [OwnPass] in ValueInstance value);
        }
    }
}