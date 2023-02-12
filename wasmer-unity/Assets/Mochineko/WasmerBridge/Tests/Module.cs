using System;
using System.Runtime.InteropServices;

namespace Mochineko.WasmerBridge.Tests
{
    public sealed class Module : IDisposable
    {
        public string Name { get; }
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

        public Module(Store store, string name, WasmByteArray binary)
        {
            if (store is null)
            {
                throw new ArgumentNullException(nameof(store));
            }

            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (binary.size == 0)
            {
                throw new ArgumentNullException(nameof(binary));
            }

            var moduleHandle = WasmAPIs.wasm_module_new(store.Handle, binary);
            if (moduleHandle == IntPtr.Zero)
            {
                Marshal.FreeHGlobal(moduleHandle);
                throw new InvalidOperationException("Failed to create module.");
            }

            this.Name = name;
            this.handle = new NativeHandle(moduleHandle);

            // TODO: Import and Export
        }

        public void Dispose()
        {
            handle.Dispose();
        }

        public Instance Instantiate(Store store, Module module, ImportObject importObject)
        {
            throw new NotImplementedException();
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
                WasmAPIs.wasm_module_delete(handle);
                return true;
            }
        }

        private static class WasmAPIs
        {
            [DllImport(NativePlugin.LibraryName)]
            public static extern IntPtr wasm_module_new(Store.NativeHandle store, in WasmByteArray binary);

            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_module_delete(IntPtr module);
        }
    }
}