using System;
using System.Runtime.InteropServices;

namespace Mochineko.WasmerBridge
{
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
        
        public Store(Engine engine)
        {
            if (engine is null)
            {
                throw new ArgumentNullException(nameof(engine));
            }
            
            handle = new NativeHandle(WasmAPIs.wasm_store_new(engine.Handle));
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
            public static extern IntPtr wasm_store_new(Engine.NativeHandle engine);

            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_store_delete(IntPtr store);
        }
    }
}