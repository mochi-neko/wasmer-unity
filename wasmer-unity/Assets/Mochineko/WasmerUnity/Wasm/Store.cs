using System;
using System.Runtime.InteropServices;
using Mochineko.WasmerUnity.Wasm.Attributes;
using Microsoft.Win32.SafeHandles;

namespace Mochineko.WasmerUnity.Wasm
{
    [OwnPointed]
    public sealed class Store : IDisposable
    {
        [return: OwnReceive]
        public static Store New(Engine engine)
        {
            if (engine is null)
            {
                throw new ArgumentNullException(nameof(engine));
            }

            return new Store(WasmAPIs.wasm_store_new(engine.Handle), hasOwnership: true);
        }

        [return: OwnReceive]
        public static Store Default()
        {
            var engine = Engine.New();

            return new Store(WasmAPIs.wasm_store_new(engine.Handle), hasOwnership: true, engine);
        }

        private Store(IntPtr handle, bool hasOwnership)
        {
            this.handle = new NativeHandle(handle, hasOwnership);
        }

        private Store(IntPtr handle, bool hasOwnership, Engine engine)
        {
            this.handle = new NativeHandle(handle, hasOwnership);
            this.engine = engine;
        }

        public void Dispose()
        {
            handle.Dispose();
            engine?.Dispose();
        }

        private readonly NativeHandle handle;
        private readonly IDisposable engine;

        internal NativeHandle Handle
        {
            get
            {
                if (handle.IsInvalid)
                {
                    throw new ObjectDisposedException(typeof(Store).FullName);
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
                WasmAPIs.wasm_store_delete(handle);
                return true;
            }
        }

        private static class WasmAPIs
        {
            [DllImport(NativePlugin.LibraryName)]
            [return: OwnReceive]
            public static extern IntPtr wasm_store_new(
                Engine.NativeHandle engine);

            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_store_delete(
                [OwnPass] [In] IntPtr handle);
        }
    }
}