using System;
using System.Runtime.InteropServices;
using Mochineko.WasmerBridge.Attributes;

namespace Mochineko.WasmerBridge
{
    [OwnStruct]
    [StructLayout(LayoutKind.Sequential)]
    internal readonly struct ValueInstance : IDisposable
    {
        private readonly byte kind;
        public ValueKind Kind => (ValueKind)kind;

        private readonly ValueUnion of;
        internal int? OfInt32 => Kind is ValueKind.Int32 ? of.i32 : null;
        internal long? OfInt64 => Kind is ValueKind.Int64 ? of.i64 : null;
        internal float? OfFloat32 => Kind is ValueKind.Float32 ? of.f32 : null;
        internal double? OfFloat64 => Kind is ValueKind.Float64 ? of.f64 : null;
        internal IntPtr? OfReference => Kind is ValueKind.AnyRef or ValueKind.FuncRef ? of.reference : null;

        public static ValueInstance NewInt32(int int32)
            => new ValueInstance(int32);

        public static ValueInstance NewInt64(long int64)
            => new ValueInstance(int64);

        public static ValueInstance NewFloat32(float float32)
            => new ValueInstance(float32);

        public static ValueInstance NewFloat64(double float64)
            => new ValueInstance(float64);

        public static ValueInstance NewAnyReference(IntPtr reference)
            => new ValueInstance(reference, false);

        public static ValueInstance NewFunctionReference(IntPtr reference)
            => new ValueInstance(reference, true);

        private ValueInstance(int int32)
        {
            this.kind = (byte)ValueKind.Int32;
            this.of = new ValueUnion(int32);
        }

        private ValueInstance(long int64)
        {
            this.kind = (byte)ValueKind.Int64;
            this.of = new ValueUnion(int64);
        }

        private ValueInstance(float float32)
        {
            this.kind = (byte)ValueKind.Float32;
            this.of = new ValueUnion(float32);
        }

        private ValueInstance(double float64)
        {
            this.kind = (byte)ValueKind.Float64;
            this.of = new ValueUnion(float64);
        }

        private ValueInstance(IntPtr reference, bool isFunction)
        {
            this.kind = (byte)(isFunction ? ValueKind.FuncRef : ValueKind.AnyRef);
            this.of = new ValueUnion(reference);
        }

        public void Dispose()
        {
            WasmAPIs.wasm_val_delete(in this);
        }

        private static class WasmAPIs
        {
            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_val_delete(
                [OwnPass] in ValueInstance handle);

            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_val_copy(
                [OwnOut] out ValueInstance copy,
                [OwnPass] in ValueInstance value);
        }
    }
}