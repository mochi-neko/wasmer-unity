using System;
using System.Runtime.InteropServices;

namespace Mochineko.WasmerBridge
{
    internal static class Wat2Wasm
    {
        public static NativeByteArray ToWasm(this string wat)
        {
            if (string.IsNullOrEmpty(wat))
            {
                throw new ArgumentNullException(nameof(wat));
            }

            using var watBinary = NativeByteArray.CreateFromManaged(System.Text.Encoding.UTF8.GetBytes(wat));
            if (watBinary.size == 0)
            {
                throw new InvalidOperationException("Failed to get wat binary as native vector.");
            }

            WasmerAPIs.wat2wasm(watBinary, out var wasm);
            if (wasm.size == 0)
            {
                wasm.Dispose();
                // TODO:
                //throw WasmerException.GetInnerLastError();
                throw new Exception("Failed to convert wat to wasm.");
            }

            return wasm;
        }

        private static class WasmerAPIs
        {
            [DllImport(NativePlugin.LibraryName)]
            public static extern void wat2wasm(in NativeByteArray wat, out NativeByteArray native);
        }
    }
}