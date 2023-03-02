using System;
using System.Runtime.InteropServices;

namespace Mochineko.WasmerUnity.Wasm
{
    [StructLayout(LayoutKind.Sequential)]
    internal readonly struct Limits : IEquatable<Limits>
    {
        public readonly uint min;
        public readonly uint max;

        public const uint MaxDefault = 0xffffffff;
        
        public Limits(uint max, uint min)
        {
            if (max < min)
            {
                throw new ArgumentOutOfRangeException($"{nameof(max)}:{max} must be greater than {nameof(min)}:{min}");
            }
            
            this.max = max;
            this.min = min;
        }

        public bool Equals(Limits other)
        {
            return min == other.min && max == other.max;
        }

        public override bool Equals(object obj)
        {
            return obj is Limits other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(min, max);
        }
    }
}