using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using Mochineko.WasmerBridge.Attributes;

namespace Mochineko.WasmerBridge
{
    [OwnPointed]
    internal sealed class FunctionType : IDisposable
    {
        internal ReadOnlySpan<ValueKind> Parameters
        {
            get
            {
                if (handle.IsInvalid)
                {
                    throw new ObjectDisposedException(typeof(FunctionType).FullName);
                }

                unsafe
                {
                    var ptr = WasmAPIs.wasm_functype_params(handle);
                    ptr->ToKinds(out var kinds);
                    return kinds;
                }
            }
        }
        
        internal ReadOnlySpan<ValueKind> Results
        {
            get
            {
                if (handle.IsInvalid)
                {
                    throw new ObjectDisposedException(typeof(FunctionType).FullName);
                }

                unsafe
                {
                    var ptr = WasmAPIs.wasm_functype_results(handle);
                    ptr->ToKinds(out var kinds);
                    return kinds;
                }
            }
        }
        
        internal static FunctionType New(in ReadOnlySpan<ValueKind> parameters, in ReadOnlySpan<ValueKind> results)
        {
            // Passes vectors ownerships to native, then vectors are released by owner:FunctionType.
            ValueTypeVector.New(in parameters, out var parametersVector);
            ValueTypeVector.New(in results, out var resultsVector);

            return new FunctionType(WasmAPIs.wasm_functype_new(in parametersVector, resultsVector));
        }

        private static FunctionType New(in ValueTypeVector parameters, in ValueTypeVector results)
        {
            return new FunctionType(WasmAPIs.wasm_functype_new(in parameters, results));
        }

        internal FunctionType(IntPtr handle)
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
                    throw new ObjectDisposedException(typeof(FunctionType).FullName);
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
            public static extern void wasm_functype_delete([OwnPass] IntPtr externalType);

            [DllImport(NativePlugin.LibraryName)]
            [return: OwnReceive]
            public static extern IntPtr wasm_functype_copy([Const] NativeHandle externalType);
            
            [DllImport(NativePlugin.LibraryName)]
            [return: Const]
            public static extern unsafe ValueTypeVector* wasm_functype_params([Const] NativeHandle functionType);
            
            [DllImport(NativePlugin.LibraryName)]
            [return: Const]
            public static extern unsafe ValueTypeVector* wasm_functype_results([Const] NativeHandle functionType);
        }
    }
}