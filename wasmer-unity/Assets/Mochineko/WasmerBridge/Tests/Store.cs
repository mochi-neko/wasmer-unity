using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace Mochineko.WasmerBridge.Tests
{
    public sealed class Store : IDisposable
    {
        private readonly NativeHandle handle;
        
        public Store(Engine engine)
        {
            handle = new NativeHandle(NativeAPIs.wasm_store_new(engine.Handle));
        }

        public void Dispose()
        {
            handle.Dispose();
        }
        
        private sealed class NativeHandle : SafeHandleZeroOrMinusOneIsInvalid
        {
            public NativeHandle(IntPtr handle) : base(true)
            {
                SetHandle(handle);
            }

            protected override bool ReleaseHandle()
            {
                NativeAPIs.wasm_store_delete(handle);
                return true;
            }
        }
        
        private static class NativeAPIs
        {
            [DllImport(NativePlugin.LibraryName)]
            public static extern IntPtr wasm_store_new(Engine.NativeHandle engine);

            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_store_delete(IntPtr store);
        }
    }
}