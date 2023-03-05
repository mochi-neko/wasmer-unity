using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using Mochineko.WasmerUnity.Wasm.Attributes;
using Mochineko.WasmerUnity.Wasm.Types;

namespace Mochineko.WasmerUnity.Wasm.Instances
{
    [OwnPointed]
    public sealed class TableInstance : IDisposable
    {
        [OwnReceive]
        public TableType Type
            => TableType.FromPointer(
                WasmAPIs.wasm_table_type(Handle),
                hasOwnership: true);

        public uint Size
            => WasmAPIs.wasm_table_size(Handle);
        
        // TODO: Implement new & get & set

        internal static TableInstance FromPointer(IntPtr ptr, bool hasOwnership)
            => new TableInstance(ptr, hasOwnership);

        private TableInstance(IntPtr handle, bool hasOwnership)
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
                    throw new ObjectDisposedException(typeof(TableInstance).FullName);
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
                WasmAPIs.wasm_table_delete(handle);
                return true;
            }
        }

        private static class WasmAPIs
        {
            [DllImport(NativePlugin.LibraryName)]
            [return: OwnReceive]
            public static extern IntPtr wasm_table_new(
                Store.NativeHandle store,
                [Const] TableType.NativeHandle type,
                IntPtr init);

            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_table_delete(
                [OwnPass] [In] IntPtr handle);

            [DllImport(NativePlugin.LibraryName)]
            [return: OwnReceive]
            public static extern IntPtr wasm_table_type(
                [Const] NativeHandle handle);

            [DllImport(NativePlugin.LibraryName)]
            [return: OwnReceive]
            public static extern IntPtr wasm_table_get(
                [Const] NativeHandle handle,
                uint index);

            [DllImport(NativePlugin.LibraryName)]
            public static extern bool wasm_table_set(
                NativeHandle handle,
                uint index,
                IntPtr element);
            
            [DllImport(NativePlugin.LibraryName)]
            public static extern uint wasm_table_size(
                [Const] NativeHandle handle);
            
            [DllImport(NativePlugin.LibraryName)]
            public static extern bool wasm_table_grow(
                [Const] NativeHandle handle,
                uint delta,
                IntPtr init);
        }
    }
}