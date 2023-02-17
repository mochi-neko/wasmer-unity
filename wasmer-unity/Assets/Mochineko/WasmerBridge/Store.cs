using System;
using System.Runtime.InteropServices;
using Mochineko.WasmerBridge.OwnAttributes;

namespace Mochineko.WasmerBridge
{
    [OwnReference]
    public sealed class Store : IDisposable
    {
        private readonly NativeHandle handle;
        
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

        private Store(IntPtr handle)
        {
            this.handle = new NativeHandle(handle);
        }
        
        public static Store New(Engine engine)
        {
            if (engine is null)
            {
                throw new ArgumentNullException(nameof(engine));
            }
            
            return new Store(WasmAPIs.wasm_store_new(engine.Handle));
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
                WasmAPIs.wasm_store_delete(handle);
                return true;
            }
        }
        
        private static class WasmAPIs
        {
            [DllImport(NativePlugin.LibraryName)]
            [return: OwnResult]
            public static extern IntPtr wasm_store_new(Engine.NativeHandle engine);

            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_store_delete([OwnParameter]IntPtr store);
        }
    }
}