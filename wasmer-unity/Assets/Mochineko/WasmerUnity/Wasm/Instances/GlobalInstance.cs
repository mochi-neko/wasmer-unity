using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using Mochineko.WasmerUnity.Wasm.Attributes;
using Mochineko.WasmerUnity.Wasm.Types;

namespace Mochineko.WasmerUnity.Wasm.Instances
{
    [OwnPointed]
    public sealed class GlobalInstance : IDisposable
    {
        [OwnReceive]
        public GlobalType Type
            => GlobalType.FromPointer(
                WasmAPIs.wasm_global_type(Handle),
                hasOwnership: true);

        public ValueInstance Get()
        {
            WasmAPIs.wasm_global_get(Handle, out var value);

            return value;
        }

        public void Set(in ValueInstance value)
        {
            using var type = Type;
            if (type.Mutability == Mutability.Constant)
            {
                throw new InvalidOperationException($"Cannot set value to constant global instance.");
            }

            WasmAPIs.wasm_global_set(Handle, in value);
        }

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