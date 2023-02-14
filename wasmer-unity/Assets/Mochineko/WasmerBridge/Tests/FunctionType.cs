using System;
using System.Runtime.InteropServices;

namespace Mochineko.WasmerBridge.Tests
{
    internal sealed class FunctionType : IDisposable
    {
        private ValueTypeArray Parameters { get; }
        private ValueTypeArray Results { get; }
        
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

        internal FunctionType(ValueTypeArray parameters, ValueTypeArray results)
        {
            // TODO: Validation
            
            this.Parameters = parameters;
            this.Results = results;
            
            handle = new NativeHandle(WasmAPIs.wasm_functype_new(parameters, results));
        }

        public void Dispose()
        {
            handle.Dispose();
        }

        internal sealed class NativeHandle : SafeHandle
        {
            public NativeHandle(IntPtr handle)
                : base(IntPtr.Zero,true)
            {
                SetHandle(handle);
            }

            public override bool IsInvalid
                => handle == IntPtr.Zero;

            protected override bool ReleaseHandle()
            {
                WasmAPIs.wasm_functype_delete(handle);
                return true;
            }
        }

        private static class WasmAPIs
        {
            [DllImport(NativePlugin.LibraryName)]
            public static extern IntPtr wasm_functype_new(ValueTypeArray parameters, ValueTypeArray results);
            
            [DllImport(NativePlugin.LibraryName)]
            public static extern ValueTypeArray wasm_functype_params(in NativeHandle functionType);
            
            [DllImport(NativePlugin.LibraryName)]
            public static extern ValueTypeArray wasm_functype_results(in NativeHandle functionType);

            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_functype_delete(IntPtr functionType);
        }
    }
}