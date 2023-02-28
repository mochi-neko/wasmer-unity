using System;
using System.Runtime.InteropServices;
using Mochineko.WasmerUnity.Wasm;
using Mochineko.WasmerUnity.Wasm.Attributes;

namespace Mochineko.WasmerUnity.Wasmer
{
    public static class Wat2Wasm
    {
        public static ReadOnlySpan<byte> WatToWasm(this string wat)
        {
            if (string.IsNullOrEmpty(wat))
            {
                throw new ArgumentNullException(nameof(wat));
            }

            ByteVector.FromText(wat, out var watVector);
            using (watVector)
            {
                if (watVector.size == 0)
                {
                    throw new InvalidOperationException("Failed to get wat binary as native vector.");
                }

                WasmerAPIs.wat2wasm(in watVector, out var wasm);
                using (wasm)
                {
                    if (wasm.size != 0)
                    {
                        wasm.ToManaged(out var managed);
                        return managed;
                    }
                    else
                    {
                        // TODO: Detailed error handling
                        throw new InvalidOperationException("Failed to convert wat to wasm.");
                    }
                }
            }
        }

        private static class WasmerAPIs
        {
            [DllImport(NativePlugin.LibraryName)]
            public static extern void wat2wasm(
                [ConstVector] in ByteVector wat,
                [OwnOut] out ByteVector wasm);
        }
    }
}