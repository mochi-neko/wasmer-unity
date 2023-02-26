using System;
using System.Runtime.InteropServices;
using Mochineko.WasmerUnity.Wasm.Attributes;
using Microsoft.Win32.SafeHandles;

namespace Mochineko.WasmerUnity.Wasm
{
    [OwnPointed]
    internal sealed class FunctionInstance : IDisposable
    {
        [OwnReceive]
        internal FunctionType Type
            => FunctionType.FromPointer(
                WasmAPIs.wasm_func_type(Handle),
                hasOwnership: true);

        internal nuint ParametersArity
            => WasmAPIs.wasm_func_param_arity(Handle);

        internal nuint ResultsArity
            => WasmAPIs.wasm_func_result_arity(Handle);

        // TODO: Make wrapper
        [return: OwnReceive]
        internal static FunctionInstance New(
            Store store,
            FunctionType type,
            FunctionCallback callback)
        {
            return new FunctionInstance(
                WasmAPIs.wasm_func_new(store.Handle, type.Handle, callback),
                hasOwnership: true);
        }

        [return: OwnReceive]
        internal static FunctionInstance NewWithEnvironment(
            Store store,
            FunctionType type,
            FunctionCallbackWithEnvironment callback,
            IntPtr environment,
            Finalizer finalizer)
        {
            return new FunctionInstance(
                WasmAPIs.wasm_func_new_with_env(
                    store.Handle, type.Handle, callback, environment, finalizer),
                hasOwnership: true);
        }

        [return: OwnReceive]
        internal Trap Call(in ValueInstanceVector arguments, ref ValueInstanceVector results)
        {
            var trapPointer = WasmAPIs.wasm_func_call(Handle, in arguments, ref results);

            // Succeeded
            if (trapPointer == IntPtr.Zero)
            {
                return null;
            }
            // Failed
            else
            {
                return Trap.FromPointer(trapPointer, true);
            }
        }

        internal static FunctionInstance FromPointer(IntPtr ptr, bool hasOwnership)
            => new FunctionInstance(ptr, hasOwnership);

        private FunctionInstance(IntPtr handle, bool hasOwnership)
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
                    throw new ObjectDisposedException(typeof(FunctionInstance).FullName);
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
                WasmAPIs.wasm_func_delete(handle);
                return true;
            }
        }

        internal unsafe delegate IntPtr FunctionCallback(
            [Const] [In] ValueInstanceVector* arguments,
            [OwnPass] ValueInstanceVector* results);

        internal unsafe delegate IntPtr FunctionCallbackWithEnvironment(
            IntPtr environment,
            [Const] ValueInstanceVector* arguments,
            [OwnPass] ValueInstanceVector* results);

        public delegate void Finalizer(
            IntPtr environment);

        private static class WasmAPIs
        {
            [DllImport(NativePlugin.LibraryName)]
            [return: OwnReceive]
            public static extern IntPtr wasm_func_new(
                Store.NativeHandle store,
                [Const] FunctionType.NativeHandle functionType,
                FunctionCallback callback);

            [DllImport(NativePlugin.LibraryName)]
            [return: OwnReceive]
            public static extern IntPtr wasm_func_new_with_env(
                Store.NativeHandle store,
                [Const] FunctionType.NativeHandle functionType,
                FunctionCallbackWithEnvironment callback,
                IntPtr environment,
                Finalizer finalizer);

            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_func_delete(
                [OwnPass] [In] IntPtr handle);

            [DllImport(NativePlugin.LibraryName)]
            [return: OwnReceive]
            public static extern IntPtr wasm_func_type(
                [Const] NativeHandle handle);

            [DllImport(NativePlugin.LibraryName)]
            public static extern nuint wasm_func_param_arity(
                [Const] NativeHandle handle);

            [DllImport(NativePlugin.LibraryName)]
            public static extern nuint wasm_func_result_arity(
                [Const] NativeHandle handle);

            [DllImport(NativePlugin.LibraryName)]
            [return: OwnReceive]
            public static extern IntPtr wasm_func_call(
                [Const] NativeHandle handle,
                [Const] in ValueInstanceVector arguments,
                [Const] ref ValueInstanceVector results);
        }
    }
}