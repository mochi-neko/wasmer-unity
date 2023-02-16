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
        public void CreateFunctionTypeTest()
        {
            using var parameters = ValueTypeVector.New();
            using var results = ValueTypeVector.New();
            
            using var functionType = FunctionType.New(parameters, results);
            functionType.ParameterTypes.size.Should().Be((nuint)0);
            functionType.ResultTypes.size.Should().Be((nuint)0);
        }
    }
    
    [StructLayout(LayoutKind.Sequential)]
    internal readonly struct FunctionType
    {
        private readonly ValueTypeVector.NativeHandle parameterTypes;
        private readonly ValueTypeVector.NativeHandle resultTypes;
        
        internal static NativeHandle New(ValueTypeVector.NativeHandle parameters, ValueTypeVector.NativeHandle results)
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
                WasmAPIs.wasm_functype_delete(handle);
                return true;
            }

            public ValueTypeVector.NativeHandle ParameterTypes
            {
                get
                {
                    if (IsInvalid)
                    {
                        throw new ObjectDisposedException(typeof(ValueType).FullName);
                    }

                    return WasmAPIs.wasm_functype_params(this);
                }
            }
            
            public ValueTypeVector.NativeHandle ResultTypes
            {
                get
                {
                    if (IsInvalid)
                    {
                        throw new ObjectDisposedException(typeof(ValueType).FullName);
                    }

                    return WasmAPIs.wasm_functype_results(this);
                }
            }
        }

        private static class WasmAPIs
        {
            [DllImport(NativePlugin.LibraryName)]
            public static extern IntPtr wasm_functype_new(ValueTypeVector.NativeHandle parameters, ValueTypeVector.NativeHandle results);
            
            [DllImport(NativePlugin.LibraryName)]
            public static extern ValueTypeVector.NativeHandle wasm_functype_params(NativeHandle functionType);
            
            [DllImport(NativePlugin.LibraryName)]
            public static extern ValueTypeVector.NativeHandle wasm_functype_results(NativeHandle functionType);

            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_functype_delete(in IntPtr functionType);
        }
    }
}