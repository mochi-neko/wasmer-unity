using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using Mochineko.WasmerBridge.Attributes;

namespace Mochineko.WasmerBridge
{
    [OwnPointed]
    internal sealed class GlobalInstance : IDisposable
    {
        [OwnReceive]
        internal GlobalType Type
            => GlobalType.FromPointer(
                WasmAPIs.wasm_global_type(Handle),
                hasOwnership: true);

        internal void Get(out ValueInstance value)
            => WasmAPIs.wasm_global_get(Handle, out value);

        internal void Set(in ValueInstance value)
            => WasmAPIs.wasm_global_set(Handle, in value);

        [return: OwnReceive]
        internal static GlobalInstance New(Store store, GlobalType type, in ValueInstance value)
        {
            if (type.Content.Kind is ValueKind.AnyRef or ValueKind.FuncRef)
            {
                throw new NotImplementedException($"Native Wasm API does not implement reference type of Global.");
            }
            
            return new GlobalInstance(
                WasmAPIs.wasm_global_new(store.Handle, type.Handle, in value),
                hasOwnership: true);
        }

        internal static GlobalInstance FromPointer(IntPtr ptr, bool hasOwnership)
            => new GlobalInstance(ptr, hasOwnership);

        private GlobalInstance(IntPtr handle, bool hasOwnership)
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
                    throw new ObjectDisposedException(typeof(GlobalInstance).FullName);
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
                WasmAPIs.wasm_global_delete(handle);
                return true;
            }
        }

        private static class WasmAPIs
        {
            [DllImport(NativePlugin.LibraryName)]
            [return: OwnReceive]
            public static extern IntPtr wasm_global_new(
                Store.NativeHandle store,
                [Const] GlobalType.NativeHandle type,
                [Const] in ValueInstance value);

            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_global_delete(
                [OwnPass] [In] IntPtr handle);

            [DllImport(NativePlugin.LibraryName)]
            [return: OwnReceive]
            public static extern IntPtr wasm_global_type(
                [Const] NativeHandle handle);

            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_global_get(
                [Const] NativeHandle handle,
                [OwnOut] out ValueInstance value);

            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_global_set(
                NativeHandle handle,
                [Const] in ValueInstance value);
        }
    }
}