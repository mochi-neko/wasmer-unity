using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using Mochineko.WasmerBridge.OwnAttributes;

namespace Mochineko.WasmerBridge
{
    [OwnReference]
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

        private Config(IntPtr handle)
        {
            this.handle = new NativeHandle(handle);
        }

        public static Config New()
        {
            return new Config(WasmAPIs.wasm_config_new());
        }

        public void Dispose()
        {
            handle.Dispose();
        }

        internal sealed class NativeHandle : SafeHandleZeroOrMinusOneIsInvalid
        {
            public NativeHandle(IntPtr handle) : base(true)
            {
                this.handle = handle;
            }

            protected override bool ReleaseHandle()
            {
                WasmAPIs.wasm_config_delete(handle);
                return true;
            }
        }

        private static class WasmAPIs
        {
            [DllImport(NativePlugin.LibraryName)]
            [return: OwnResult]
            public static extern IntPtr wasm_config_new();

            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_config_delete([OwnParameter] IntPtr config);
        }
    }
}