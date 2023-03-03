using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using Mochineko.WasmerUnity.Wasm.Attributes;

namespace Mochineko.WasmerUnity.Wasm.Types
{
    [OwnPointed]
    public sealed class TableType : IDisposable
    {
        internal Limits Limits
        {
            get
            {
                unsafe
                {
                    return *WasmAPIs.wasm_tabletype_limits(Handle);
                }
            }
        }

        internal ValueType Element
            => ValueType.FromPointer(
                WasmAPIs.wasm_tabletype_element(Handle),
                hasOwnership: false);

        [return: OwnReceive]
        internal static TableType New([OwnPass] ValueType element, in Limits limits)
        {
            var handle = WasmAPIs.wasm_tabletype_new(element.Handle, limits);

            // Passes ownership to native.
            element.Handle.SetHandleAsInvalid();

            return new TableType(handle, hasOwnership: true);
        }

        internal static TableType FromPointer(IntPtr ptr, bool hasOwnership)
            => new TableType(ptr, hasOwnership);

        private TableType(IntPtr handle, bool hasOwnership)
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
                    throw new ObjectDisposedException(typeof(TableType).FullName);
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
                WasmAPIs.wasm_tabletype_delete(handle);
                return true;
            }
        }

        private static class WasmAPIs
        {
            [DllImport(NativePlugin.LibraryName)]
            [return: OwnReceive]
            public static extern IntPtr wasm_tabletype_new(
                [OwnPass] ValueType.NativeHandle valueType,
                [Const] in Limits limits);

            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_tabletype_delete(
                [OwnPass] [In] IntPtr handle);

            [DllImport(NativePlugin.LibraryName)]
            [return: Const]
            public static extern IntPtr wasm_tabletype_element(
                [Const] NativeHandle handle);

            [DllImport(NativePlugin.LibraryName)]
            [return: Const]
            public static extern unsafe Limits* wasm_tabletype_limits(
                [Const] NativeHandle handle);
        }
    }
}