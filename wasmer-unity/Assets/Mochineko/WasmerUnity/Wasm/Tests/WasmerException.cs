using System;
using System.Runtime.InteropServices;
using Mochineko.WasmerUnity.Wasm;

namespace Mochineko.WasmerUnity.Wasm.Tests
{
    public sealed class WasmerException : Exception
    {
        public WasmerException(string message) : base(message) { }
        
        internal static WasmerException GetInnerLastError()
        {
            var length = WasmerAPIs.wasmer_last_error_length();

            var message = new char[length];

            WasmerAPIs.wasmer_last_error_message(Marshal.UnsafeAddrOfPinnedArrayElement(message, 0), length);

            return new WasmerException(new string(message));
        }
        
        private static class WasmerAPIs
        {
            [DllImport(NativePlugin.LibraryName)]
            public static extern int wasmer_last_error_length();
            
            [DllImport(NativePlugin.LibraryName)]
            public static extern int wasmer_last_error_message(IntPtr buffer, int length);
        }
    }
}