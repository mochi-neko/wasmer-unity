using System;
using System.Runtime.InteropServices;
using FluentAssertions;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace Mochineko.WasmerBridge.Tests
{
    [TestFixture]
    internal sealed class FunctionTypeTest
    {
        [Test, RequiresPlayMode(false)]
        [Ignore("Remains crashes")]
        public void CreateEmptyFunctionTypeTest()
        {
            using var parameters = ValueTypeVector.NewEmpty();
            using var results = ValueTypeVector.NewEmpty();
            
            // using var explicitEmptyFunctionType = FunctionType.New(parameters.DangerousGetHandle(), results.DangerousGetHandle());
            // explicitEmptyFunctionType.ParameterTypes.size.Should().Be((nuint)0);
            // explicitEmptyFunctionType.ResultTypes.size.Should().Be((nuint)0);
        }
    }
    
    internal sealed class FunctionType
    {
        internal readonly IntPtr parameterTypes;
        internal readonly IntPtr resultTypes;
        
        // internal static NativeHandle New(ValueTypeVector.NativeHandle parameters, ValueTypeVector.NativeHandle results)
        // {
        //     return new NativeHandle(WasmAPIs.wasm_functype_new(parameters, results));
        // }
        internal static NativeHandle New(IntPtr parameters, IntPtr results)
        {
            return new NativeHandle(WasmAPIs.wasm_functype_new(parameters, results));
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
                //WasmAPIs.wasm_functype_delete(handle);
                return true;
            }

            // public ValueTypeVector.NativeHandle ParameterTypes
            // {
            //     get
            //     {
            //         if (IsInvalid)
            //         {
            //             throw new ObjectDisposedException(typeof(FunctionType).FullName);
            //         }
            //
            //         return ValueTypeVector.FromPointer(WasmAPIs.wasm_functype_params(this));
            //     }
            // }
            
            // public ValueTypeVector.NativeHandle ResultTypes
            // {
            //     get
            //     {
            //         if (IsInvalid)
            //         {
            //             throw new ObjectDisposedException(typeof(FunctionType).FullName);
            //         }
            //
            //         return ValueTypeVector.FromPointer(WasmAPIs.wasm_functype_results(this));
            //     }
            // }
        }

        private static class WasmAPIs
        {
            // [DllImport(NativePlugin.LibraryName)]
            // public static extern IntPtr wasm_functype_new(ValueTypeVector.NativeHandle parameters, ValueTypeVector.NativeHandle results);
            [DllImport(NativePlugin.LibraryName)]
            public static extern IntPtr wasm_functype_new(IntPtr parameters, IntPtr results);

            [DllImport(NativePlugin.LibraryName)]
            public static extern IntPtr wasm_functype_params(NativeHandle functionType);
            
            [DllImport(NativePlugin.LibraryName)]
            public static extern IntPtr wasm_functype_results(NativeHandle functionType);

            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_functype_delete(in IntPtr functionType);
        }
    }
}