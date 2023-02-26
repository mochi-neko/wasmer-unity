using System;
using System.Runtime.InteropServices;
using Mochineko.WasmerUnity.Wasm.Attributes;
using Microsoft.Win32.SafeHandles;

namespace Mochineko.WasmerUnity.Wasm
{
    [OwnPointed]
    internal sealed class FunctionType : IDisposable
    {
        internal ReadOnlySpan<ValueKind> Parameters
        {
            get
            {
                unsafe
                {
                    var ptr = WasmAPIs.wasm_functype_params(Handle);
                    ptr->ToKinds(out var kinds);
                    return kinds;
                }
            }
        }

        internal ReadOnlySpan<ValueKind> Results
        {
            get
            {
                unsafe
                {
                    var ptr = WasmAPIs.wasm_functype_results(Handle);
                    ptr->ToKinds(out var kinds);
                    return kinds;
                }
            }
        }

        [return: OwnReceive]
        internal static FunctionType New(in ReadOnlySpan<ValueKind> parameters, in ReadOnlySpan<ValueKind> results)
        {
            // Passes vectors ownerships to native, then vectors are released by owner:FunctionType.
            ValueTypeVector.New(in parameters, out var parametersVector);
            ValueTypeVector.New(in results, out var resultsVector);

            return New(in parametersVector, in resultsVector);
        }

        [return: OwnReceive]
        private static FunctionType New([OwnPass] in ValueTypeVector parameters, [OwnPass] in ValueTypeVector results)
        {
            return new FunctionType(
                WasmAPIs.wasm_functype_new(in parameters, in results),
                hasOwnership: true);
        }

        internal static FunctionType FromPointer(IntPtr ptr, bool hasOwnership)
            => new FunctionType(ptr, hasOwnership);

        private FunctionType(IntPtr handle, bool hasOwnership)
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
                    throw new ObjectDisposedException(typeof(FunctionType).FullName);
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
                WasmAPIs.wasm_functype_delete(handle);
                return true;
            }
        }

        private static class WasmAPIs
        {
            [DllImport(NativePlugin.LibraryName)]
            [return: OwnReceive]
            public static extern IntPtr wasm_functype_new(
                [OwnPass] in ValueTypeVector parameters,
                [OwnPass] in ValueTypeVector results);

            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_functype_delete(
                [OwnPass] [In] IntPtr handle);

            [DllImport(NativePlugin.LibraryName)]
            [return: Const]
            public static extern unsafe ValueTypeVector* wasm_functype_params(
                [Const] NativeHandle handle);

            [DllImport(NativePlugin.LibraryName)]
            [return: Const]
            public static extern unsafe ValueTypeVector* wasm_functype_results(
                [Const] NativeHandle handle);
        }
    }
}