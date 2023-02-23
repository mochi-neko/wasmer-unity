using System;
using System.Runtime.InteropServices;
using Mochineko.WasmerBridge.Attributes;

namespace Mochineko.WasmerBridge
{
    [OwnStruct]
    [StructLayout(LayoutKind.Explicit)]
    internal readonly struct ValueUnion
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
    }
}