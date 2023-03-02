using System;
using System.Runtime.InteropServices;
using Mochineko.WasmerUnity.Wasm.Attributes;
using Microsoft.Win32.SafeHandles;

namespace Mochineko.WasmerUnity.Wasm
{
    [OwnPointed]
    public sealed class GlobalType : IDisposable
    {
        public ValueType Content
        {
            get
            {
                var ptr = WasmAPIs.wasm_globaltype_content(Handle);

                return ValueType.FromPointer(ptr, hasOwnership: false);
            }
        }

        public Mutability Mutability
            => (Mutability)WasmAPIs.wasm_globaltype_mutability(Handle);

        [return: OwnReceive]
        internal static GlobalType New([OwnPass] ValueType valueType, Mutability mutability)
        {
            var handle = WasmAPIs.wasm_globaltype_new(valueType.Handle, (byte)mutability);

            // Passes ownership to native.
            valueType.Handle.SetHandleAsInvalid();

            return new GlobalType(handle, hasOwnership: true);
        }
        
        internal static GlobalType FromPointer(IntPtr ptr, bool hasOwnership)
            => new GlobalType(ptr, hasOwnership);

        private GlobalType(IntPtr handle, bool hasOwnership)
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
                    throw new ObjectDisposedException(typeof(GlobalType).FullName);
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
                WasmAPIs.wasm_globaltype_delete(handle);
                return true;
            }
        }

        private static class WasmAPIs
        {
            [DllImport(NativePlugin.LibraryName)]
            [return: OwnReceive]
            public static extern IntPtr wasm_globaltype_new(
                [OwnPass] ValueType.NativeHandle valueType,
                byte mutability);

            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_globaltype_delete(
                [OwnPass] [In] IntPtr handle);

            [DllImport(NativePlugin.LibraryName)]
            [return: Const]
            public static extern IntPtr wasm_globaltype_content(
                [Const] NativeHandle handle);

            [DllImport(NativePlugin.LibraryName)]
            public static extern byte wasm_globaltype_mutability(
                [Const] NativeHandle handle);
        }
    }
}