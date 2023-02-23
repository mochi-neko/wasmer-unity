using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using Mochineko.WasmerBridge.Attributes;

namespace Mochineko.WasmerBridge
{
    [OwnPointed]
    internal sealed class FunctionInstance : IDisposable
    {
        internal FunctionType Type
            => FunctionType.FromPointer(WasmAPIs.wasm_func_type(Handle));

        internal nuint ParametersArity
            => WasmAPIs.wasm_func_param_arity(Handle);

        internal nuint ResultsArity
            => WasmAPIs.wasm_func_result_arity(Handle);

        internal static FunctionInstance FromPointer(IntPtr ptr)
            => new FunctionInstance(ptr);

        internal static FunctionInstance New(Store store, FunctionType type, FunctionCallback callback)
        {
            return new FunctionInstance(WasmAPIs.wasm_func_new(store.Handle, type.Handle, callback));
        }

        private FunctionInstance(IntPtr handle)
        {
            this.handle = new NativeHandle(handle);
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
                    throw new ObjectDisposedException(typeof(FunctionInstance).FullName);
                }

                return handle;
            }
        }

        internal sealed class NativeHandle : SafeHandleZeroOrMinusOneIsInvalid
        {
            public NativeHandle(IntPtr handle) : base(true)
            {
                this.handle = handle;
            }

            protected override bool ReleaseHandle()
            {
                WasmAPIs.wasm_func_delete(handle);
                return true;
            }
        }

        private static class WasmAPIs
        {
            public delegate void FinalizerDelegate(IntPtr environment);

            [DllImport(NativePlugin.LibraryName)]
            [return: OwnReceive]
            public static extern IntPtr wasm_func_new(
                Store.NativeHandle store,
                [Const] FunctionType.NativeHandle functionType,
                FunctionCallback callback);

            [DllImport(NativePlugin.LibraryName)]
            [return: OwnReceive]
            public static extern unsafe IntPtr wasm_func_new_with_env(
                Store.NativeHandle store,
                [Const] FunctionType.NativeHandle functionType,
                FunctionCallbackWithEnvironment callback,
                IntPtr environment,
                FinalizerDelegate finalizer);

            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_func_delete(
                [OwnPass] [In] IntPtr functionInstance);

            [DllImport(NativePlugin.LibraryName)]
            [return: OwnReceive]
            public static extern IntPtr wasm_func_copy(
                [Const] NativeHandle functionInstance);

            [DllImport(NativePlugin.LibraryName)]
            [return: OwnReceive]
            public static extern IntPtr wasm_func_type(
                [Const] NativeHandle functionInstance);

            [DllImport(NativePlugin.LibraryName)]
            public static extern nuint wasm_func_param_arity(
                [Const] NativeHandle functionInstance);

            [DllImport(NativePlugin.LibraryName)]
            public static extern nuint wasm_func_result_arity(
                [Const] NativeHandle functionInstance);

            [DllImport(NativePlugin.LibraryName)]
            public static extern IntPtr wasm_func_call(
                [Const] NativeHandle functionInstance,
                [Const] in ValueInstanceVector arguments,
                [Const] ref ValueInstanceVector results);
        }
    }

    internal readonly struct Reference
    {
    }

    internal sealed class FunctionCallbackWithEnvironment
    {
    }

    internal readonly struct FunctionCallback
    {
    }
}