using System.Runtime.InteropServices;

namespace Mochineko.WasmerUnity.Wasm
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct Limits
    {
        public uint min;
        public uint max;

        public const uint MaxDefault = 0xffffffff;
    }
}