using System;
using System.Runtime.InteropServices;

namespace Mochineko.WasmerBridge
{
    public sealed class ValueType : IDisposable
    {
        internal ValueKind ValueKind { get; }
        
        private readonly NativeHandle handle;

        internal NativeHandle Handle
        {
            get
            {
                if (handle.IsInvalid)
                {
                    throw new ObjectDisposedException(typeof(ValueType).FullName);
                }

                return handle;
            }
        }

        internal ValueType(ValueKind valueKind)
        {
            this.ValueKind = valueKind;
            
            handle = new NativeHandle(WasmAPIs.wasm_valtype_new(valueKind));
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
                WasmAPIs.wasm_valtype_delete(handle);
                return true;
            }
        }

        private static class WasmAPIs
        {
            [DllImport(NativePlugin.LibraryName)]
            public static extern IntPtr wasm_valtype_new(ValueKind valueKind);
            
            [DllImport(NativePlugin.LibraryName)]
            public static extern ValueKind wasm_valtype_kind(in NativeHandle valueType);

            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_valtype_delete(IntPtr importType);
        }
    }
}