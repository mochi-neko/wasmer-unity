using System;
using System.Runtime.InteropServices;

namespace Mochineko.WasmerBridge
{
    public sealed class Config : IDisposable
    {
        private readonly NativeHandle handle;

        internal NativeHandle Handle
        {
            get
            {
                if (handle.IsInvalid)
                {
                    throw new ObjectDisposedException(typeof(Config).FullName);
                }

                return handle;
            }
        }
        
        public Config()
        {
            handle = new NativeHandle(WasmAPIs.wasm_config_new());
        }
        
        public void Dispose()
        {
            handle.Dispose();
        }
        
        internal sealed class NativeHandle : SafeHandle
        {
            public NativeHandle(IntPtr handle)
                : base(IntPtr.Zero,true)
            {
                SetHandle(handle);
            }

            public override bool IsInvalid
                => handle == IntPtr.Zero;

            protected override bool ReleaseHandle()
            {
                WasmAPIs.wasm_config_delete(handle);
                return true;
            }
        }
        
        private static class WasmAPIs
        {
            [DllImport(NativePlugin.LibraryName)]
            public static extern IntPtr wasm_config_new();

            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_config_delete(IntPtr config);
        }
    }
}