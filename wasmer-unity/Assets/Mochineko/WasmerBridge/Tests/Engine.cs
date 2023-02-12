using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace Mochineko.WasmerBridge.Tests
{
    public sealed class Engine : IDisposable
    {
        private readonly NativeHandle handle;

        internal NativeHandle Handle
        {
            get
            {
                if (handle.IsInvalid)
                {
                    throw new ObjectDisposedException(typeof(Engine).FullName);
                }

                return handle;
            }
        }
        
        public Engine()
        {
            handle = new NativeHandle(WasmAPIs.wasm_engine_new());
        }
        
        public void Dispose()
        {
            handle.Dispose();
        }
        
        internal sealed class NativeHandle : SafeHandleZeroOrMinusOneIsInvalid
        {
            public NativeHandle(IntPtr handle) : base(true)
            {
                SetHandle(handle);
            }

            protected override bool ReleaseHandle()
            {
                WasmAPIs.wasm_engine_delete(handle);
                return true;
            }
        }
        
        private static class WasmAPIs
        {
            [DllImport(NativePlugin.LibraryName)]
            public static extern IntPtr wasm_engine_new();

            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_engine_delete(IntPtr engine);
        }
    }
}