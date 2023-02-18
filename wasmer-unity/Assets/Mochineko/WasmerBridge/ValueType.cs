using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using Mochineko.WasmerBridge.Attributes;

namespace Mochineko.WasmerBridge
{
    [OwnPointed]
    internal sealed class ValueType : IDisposable
    {
        internal ValueKind ValueKind 
            => WasmAPIs.wasm_valtype_kind(handle);

        internal static ValueType New(ValueKind kind)
        {
            return new ValueType(WasmAPIs.wasm_valtype_new((byte)kind));
        }

        internal static ValueKind ToKind(ValueType valueType)
        {
            return WasmAPIs.wasm_valtype_kind(valueType.Handle);
        }
        
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
        
        private ValueType(IntPtr handle)
        {
            this.handle = new NativeHandle(handle);
        }

        public void Dispose()
        {
            handle.Dispose();
        }
        
        internal sealed class NativeHandle : SafeHandleZeroOrMinusOneIsInvalid
        {
            public NativeHandle(IntPtr handle)
                : base(true)
            {
                SetHandle(handle);
            }

            protected override bool ReleaseHandle()
            {
                WasmAPIs.wasm_valtype_delete(handle);
                return true;
            }
        }

        private static class WasmAPIs
        {
            [DllImport(NativePlugin.LibraryName)]
            [return: OwnResult]
            public static extern IntPtr wasm_valtype_new(byte valueKind);

            [DllImport(NativePlugin.LibraryName)]
            public static extern ValueKind wasm_valtype_kind([Const] NativeHandle valueType);

            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_valtype_delete([OwnParameter] IntPtr valueType);

            [DllImport(NativePlugin.LibraryName)]
            [return: OwnResult]
            public static extern ValueType wasm_valtype_copy(IntPtr valueType);
        }
    }
}