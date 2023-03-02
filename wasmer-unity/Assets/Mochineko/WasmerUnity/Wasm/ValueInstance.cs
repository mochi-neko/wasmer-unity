using System;
using System.Runtime.InteropServices;
using Mochineko.WasmerUnity.Wasm.Attributes;

namespace Mochineko.WasmerUnity.Wasm
{
    [OwnStruct]
    [StructLayout(LayoutKind.Sequential)]
    public readonly struct ValueInstance : IDisposable, IEquatable<ValueInstance>
    {
        private readonly byte kind;
        public ValueKind Kind => (ValueKind)kind;

        private readonly ValueUnion of;

        public int OfInt32 => Kind is ValueKind.Int32
            ? of.i32
            : throw new InvalidCastException($"ValueKind is not {ValueKind.Int32} but {Kind}.");

        public long OfInt64 => Kind is ValueKind.Int64
            ? of.i64
            : throw new InvalidCastException($"ValueKind is not {ValueKind.Int64} but {Kind}.");

        public float OfFloat32 => Kind is ValueKind.Float32
            ? of.f32
            : throw new InvalidCastException($"ValueKind is not {ValueKind.Float32} but {Kind}.");

        public double OfFloat64 => Kind is ValueKind.Float64
            ? of.f64
            : throw new InvalidCastException($"ValueKind is not {ValueKind.Float64} but {Kind}.");

        internal IntPtr OfReference => Kind is ValueKind.AnyRef or ValueKind.FuncRef
            ? of.reference
            : throw new InvalidCastException($"ValueKind is not {ValueKind.AnyRef} or {ValueKind.FuncRef} but {Kind}.");

        internal T OfType<T>()
            => (T)Of;
        
        // NOTE: Boxing via "object".
        internal object Of
            => Kind switch
            {
                ValueKind.Int32 => of.i32,
                ValueKind.Int64 => of.i64,
                ValueKind.Float32 => of.f32,
                ValueKind.Float64 => of.f64,
                ValueKind.AnyRef => of.reference,
                ValueKind.FuncRef => of.reference,
                _ => throw new ArgumentOutOfRangeException()
            };

        internal static ValueInstance New<T>(T value)
        {
            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            return value switch
            {
                int int32 => NewInt32(int32),
                long int64 => NewInt64(int64),
                float float32 => NewFloat32(float32),
                double float64 => NewFloat64(float64),
                IntPtr reference => throw new NotImplementedException(), // NewAnyReference(reference),
                _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
            };
        }

        // NOTE: Boxing via "object".
        internal static ValueInstance New(ValueKind kind, object value)
        {
            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            return kind switch
            {
                ValueKind.Int32 => value is int int32
                    ? NewInt32(int32)
                    : throw new InvalidCastException($"From {value.GetType()} to {typeof(int).FullName}"),

                ValueKind.Int64 => value is long int64
                    ? NewInt64(int64)
                    : throw new InvalidCastException($"From {value.GetType()} to {typeof(long).FullName}"),

                ValueKind.Float32 => value is float float32
                    ? NewFloat32(float32)
                    : throw new InvalidCastException($"From {value.GetType()} to {typeof(float).FullName}"),

                ValueKind.Float64 => value is double float64
                    ? NewFloat64(float64)
                    : throw new InvalidCastException($"From {value.GetType()} to {typeof(double).FullName}"),

                ValueKind.AnyRef => value is IntPtr anyReference
                    ? NewAnyReference(anyReference)
                    : throw new InvalidCastException($"From {value.GetType()} to {typeof(IntPtr).FullName}"),

                ValueKind.FuncRef => value is IntPtr functionReference
                    ? NewFunctionReference(functionReference)
                    : throw new InvalidCastException($"From {value.GetType()} to {typeof(IntPtr).FullName}"),

                _ => throw new ArgumentOutOfRangeException(nameof(kind), kind, null)
            };
        }

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
            // Should not call "delete" because "new" is not defined.  
            // WasmAPIs.wasm_val_delete(in this);
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

        public bool Equals(ValueInstance other)
        {
            return kind == other.kind && of.Equals(other.of);
        }

        public override bool Equals(object obj)
        {
            return obj is ValueInstance other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(kind, of);
        }
    }
}