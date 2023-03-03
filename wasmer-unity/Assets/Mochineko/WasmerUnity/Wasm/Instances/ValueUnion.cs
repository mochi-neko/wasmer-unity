using System;
using System.Runtime.InteropServices;
using Mochineko.WasmerUnity.Wasm.Attributes;

namespace Mochineko.WasmerUnity.Wasm.Instances
{
    [OwnStruct]
    [StructLayout(LayoutKind.Explicit)]
    internal readonly struct ValueUnion : IEquatable<ValueUnion>
    {
        [FieldOffset(0)] internal readonly int i32;
        [FieldOffset(0)] internal readonly long i64;
        [FieldOffset(0)] internal readonly float f32;
        [FieldOffset(0)] internal readonly double f64;
        [FieldOffset(0)] internal readonly IntPtr reference;

        internal ValueUnion(int i32)
        {
            this.i64 = default;
            this.f32 = default;
            this.f64 = default;
            this.reference = default;

            this.i32 = i32;
        }

        internal ValueUnion(long i64)
        {
            this.i32 = default;
            this.f32 = default;
            this.f64 = default;
            this.reference = default;

            this.i64 = i64;
        }

        internal ValueUnion(float f32)
        {
            this.i32 = default;
            this.i64 = default;
            this.f64 = default;
            this.reference = default;

            this.f32 = f32;
        }

        internal ValueUnion(double f64)
        {
            this.i32 = default;
            this.i64 = default;
            this.f32 = default;
            this.reference = default;

            this.f64 = f64;
        }

        internal ValueUnion(IntPtr reference)
        {
            this.i32 = default;
            this.i64 = default;
            this.f32 = default;
            this.f64 = default;

            this.reference = reference;
        }

        public bool Equals(ValueUnion other)
        {
            return i32 == other.i32 && i64 == other.i64 && f32.Equals(other.f32) && f64.Equals(other.f64) && reference.Equals(other.reference);
        }

        public override bool Equals(object obj)
        {
            return obj is ValueUnion other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(i32, i64, f32, f64, reference);
        }
    }
}