using System;
using System.Runtime.InteropServices;

namespace Mochineko.WasmerBridge.Tests
{
    public sealed class ImportType : IDisposable
    {
        internal string ModuleName { get; }
        internal string Name { get; }
        internal ExternType Type { get; }
        
        private readonly NativeHandle handle;

        internal NativeHandle Handle
        {
            get
            {
                if (handle.IsInvalid)
                {
                    throw new ObjectDisposedException(typeof(Module).FullName);
                }

                return handle;
            }
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
                WasmAPIs.wasm_importtype_delete(handle);
                return true;
            }
        }

        private static class WasmAPIs
        {
            [DllImport(NativePlugin.LibraryName)]
            public static extern IntPtr wasm_importtype_new(Module.NativeHandle module, NativeByteArray name, ExternType.NativeHandle type);
            
            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_importtype_delete(IntPtr importType);
        }
    }
}