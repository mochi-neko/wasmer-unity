using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using Mochineko.WasmerBridge.Attributes;

namespace Mochineko.WasmerBridge
{
    [OwnPointed]
    internal sealed class ValueType : IDisposable
    {
        internal ValueKind Kind
            => (ValueKind)WasmAPIs.wasm_valtype_kind(Handle);

        [return: OwnReceive]
        internal static ValueType New(ValueKind kind)
        {
            return new ValueType(
                WasmAPIs.wasm_valtype_new((byte)kind),
                hasOwnership: true);
        }

        internal static ValueKind KindFromPointer(IntPtr valueType)
        {
            return (ValueKind)WasmAPIs.wasm_valtype_kind(valueType);
        }

        internal static ValueType FromPointer(IntPtr pointer, bool hasOwnership)
            => new ValueType(pointer, hasOwnership);

        private ValueType(IntPtr handle, bool hasOwnership)
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
                    throw new ObjectDisposedException(typeof(ValueType).FullName);
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
                WasmAPIs.wasm_valtype_delete(handle);
                return true;
            }
        }

        private static class WasmAPIs
        {
            [DllImport(NativePlugin.LibraryName)]
            [return: OwnReceive]
            public static extern IntPtr wasm_valtype_new(
                byte valueKind);

            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_valtype_delete(
                [OwnPass] [In] IntPtr handle);

            [DllImport(NativePlugin.LibraryName)]
            public static extern byte wasm_valtype_kind(
                [Const] NativeHandle handle);

            [DllImport(NativePlugin.LibraryName)]
            public static extern byte wasm_valtype_kind(
                [Const] IntPtr valueType);
        }
    }
}