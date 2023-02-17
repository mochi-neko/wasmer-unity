using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using Mochineko.WasmerBridge.OwnAttributes;

namespace Mochineko.WasmerBridge
{
    [OwnStruct]
    [StructLayout(LayoutKind.Sequential)]
    internal readonly struct ValueType
    {
        private readonly ValueKind valueKind;

        internal static NativeHandle NewAsHandle(ValueKind kind)
        {
            return new NativeHandle(WasmAPIs.wasm_valtype_new((byte)kind));
        }
        
        internal static IntPtr New(ValueKind kind)
        {
            return WasmAPIs.wasm_valtype_new((byte)kind);
        }

        internal static ValueKind ToKind(IntPtr valueType)
        {
            return WasmAPIs.wasm_valtype_kind(valueType);
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

            public ValueKind ValueKind
            {
                get
                {
                    if (IsInvalid)
                    {
                        throw new ObjectDisposedException(typeof(ValueType).FullName);
                    }

                    return WasmAPIs.wasm_valtype_kind(this);
                }
            }
        }

        private static class WasmAPIs
        {
            [DllImport(NativePlugin.LibraryName)]
            [return: OwnResult]
            public static extern IntPtr wasm_valtype_new(byte valueKind);
            
            [DllImport(NativePlugin.LibraryName)]
            public static extern ValueKind wasm_valtype_kind(NativeHandle valueType);
            
            [DllImport(NativePlugin.LibraryName)]
            public static extern ValueKind wasm_valtype_kind(IntPtr valueType);

            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_valtype_delete([OwnParameter]IntPtr valueType);
            
            [DllImport(NativePlugin.LibraryName)]
            [return: OwnResult]
            public static extern IntPtr wasm_valtype_copy(IntPtr valueType);
        }
    }
}