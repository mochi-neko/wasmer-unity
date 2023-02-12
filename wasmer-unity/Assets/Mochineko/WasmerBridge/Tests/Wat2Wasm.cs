using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Mochineko.WasmerBridge.Tests
{
    public static class Wat2Wasm
    {
        public static WasmByteArray ToWasm(this string wat)
        {
            if (string.IsNullOrEmpty(wat))
            {
                throw new ArgumentNullException(nameof(wat));
            }

            using var watBinary = WasmByteArray.ToNativeArray(System.Text.Encoding.UTF8.GetBytes(wat));
            if (watBinary.size == 0)
            {
                throw new InvalidOperationException("Failed to get wat binary as native vector.");
            }

            WasmerAPIs.wat2wasm(watBinary, out var wasm);
            if (wasm.size == 0)
            {
                wasm.Dispose();
                throw WasmerException.GetInnerLastError();
            }

            return wasm;
        }

        private static class WasmerAPIs
        {
            [DllImport(NativePlugin.LibraryName)]
            public static extern void wat2wasm(in WasmByteArray wat, out WasmByteArray wasm);
        }
    }
}