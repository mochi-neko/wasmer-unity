using System;
using System.Runtime.InteropServices;
using Mochineko.WasmerUnity.Wasm.Attributes;
using Microsoft.Win32.SafeHandles;

namespace Mochineko.WasmerUnity.Wasm
{
    [OwnPointed]
    public sealed class Engine : IDisposable
    {
        [return: OwnReceive]
        public static Engine New()
        {
            return new Engine(
                WasmAPIs.wasm_engine_new(),
                hasOwnership: true);
        }

        [return: OwnReceive]
        public static Engine New([OwnPass] Config config)
        {
            if (config is null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            var engine = new Engine(
                WasmAPIs.wasm_engine_new_with_config(config.Handle),
                hasOwnership: true);

            // Passes ownership to native.
            config.Handle.SetHandleAsInvalid();

            return engine;
        }

        private Engine(IntPtr handle, bool hasOwnership)
        {
            this.handle = new NativeHandle(handle, hasOwnership);
        }

        public void Dispose()
        {
            handle.Dispose();
        }

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

        internal sealed class NativeHandle : SafeHandleZeroOrMinusOneIsInvalid
        {
            public NativeHandle(IntPtr handle, bool ownsHandle)
                : base(ownsHandle)
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
            [return: OwnReceive]
            public static extern IntPtr wasm_engine_new();

            [DllImport(NativePlugin.LibraryName)]
            [return: OwnReceive]
            public static extern IntPtr wasm_engine_new_with_config(
                [OwnPass] [In] Config.NativeHandle config);

            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_engine_delete(
                [OwnPass] [In] IntPtr handle);
        }
    }
}