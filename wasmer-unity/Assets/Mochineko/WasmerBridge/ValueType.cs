using System;
using System.Runtime.InteropServices;

namespace Mochineko.WasmerBridge
{
    [StructLayout(LayoutKind.Sequential)]
    internal readonly struct ValueType
    {
        internal readonly ValueKind valueKind;

        // NOTE: ValueType cannot have NativeHandle as field, but must be call "delete" function.
        public static (ValueType valueType, NativeHandle handle) New(ValueKind kind)
        {
            var ptr = WasmAPIs.wasm_valtype_new((byte)kind);
            
            var valueType = Marshal.PtrToStructure<ValueType>(ptr);

            return (valueType, new NativeHandle(ptr));
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
            public static extern IntPtr wasm_valtype_new(byte valueKind);
            
            [DllImport(NativePlugin.LibraryName)]
            public static extern ValueKind wasm_valtype_kind(NativeHandle valueType);

            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_valtype_delete(IntPtr valueType);
        }
    }
}