using System;
using System.Runtime.InteropServices;
using Mochineko.WasmerBridge.Attributes;

namespace Mochineko.WasmerBridge
{
    internal static class Wat2Wasm
    {
        internal static void ToWasm(this string wat, [OwnOut] out ByteVector wasm)
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

                WasmerAPIs.wat2wasm(in watVector, out wasm);
                if (wasm.size != 0)
                {
                    return;
                }
                else
                {
                    // TODO: Detailed error handling
                    throw new InvalidOperationException("Failed to convert wat to wasm.");
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