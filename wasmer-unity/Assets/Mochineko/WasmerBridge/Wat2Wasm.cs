using System;
using System.Runtime.InteropServices;
using Mochineko.WasmerBridge.Attributes;

namespace Mochineko.WasmerBridge
{
    internal static class Wat2Wasm
    {
        internal static ByteVector FromWatToWasm(this string wat)
        {
            if (string.IsNullOrEmpty(wat))
            {
                throw new ArgumentNullException(nameof(wat));
            }

            var watBinary = System.Text.Encoding.UTF8.GetBytes(wat);

            using var watVector = ByteVector.New(watBinary);
            if (watVector.size == 0)
            {
                throw new InvalidOperationException("Failed to get wat binary as native vector.");
            }

            WasmerAPIs.wat2wasm(in watVector, out var wasm);
            if (wasm.size == 0)
            {
                // TODO: Detailed error handling
                throw new InvalidOperationException("Failed to convert wat to wasm.");
            }

            return wasm;
        }

        private static class WasmerAPIs
        {
            [DllImport(NativePlugin.LibraryName)]
            public static extern void wat2wasm([ConstVector]in ByteVector wat, [OwnOut]out ByteVector native);
        }
    }
}