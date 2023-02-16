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
        public void CreateEmptyFunctionTypeTest()
        {
            using var functionType = FunctionType.New();
            functionType.ParameterTypes.size.Should().Be((nuint)0);
            functionType.ResultTypes.size.Should().Be((nuint)0);
            
            //using var parameters = ValueTypeVector.New();
            //using var results = ValueTypeVector.New();
            
            //using var explicitEmptyFunctionType = FunctionType.New(parameters, results);
            //explicitEmptyFunctionType.ParameterTypes.size.Should().Be((nuint)0);
            //explicitEmptyFunctionType.ResultTypes.size.Should().Be((nuint)0);
        }
    }
    
    [StructLayout(LayoutKind.Sequential)]
    internal readonly struct FunctionType
    {
        private readonly IntPtr parameterTypes;
        private readonly IntPtr resultTypes;
        
        internal static NativeHandle New()
        {
            return new NativeHandle(WasmAPIs.wasm_functype_new(IntPtr.Zero, IntPtr.Zero));
        }
        
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
                        throw new ObjectDisposedException(typeof(FunctionType).FullName);
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
                        throw new ObjectDisposedException(typeof(FunctionType).FullName);
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
            public static extern IntPtr wasm_functype_new(IntPtr parameters, IntPtr results);

            [DllImport(NativePlugin.LibraryName)]
            public static extern ValueTypeVector.NativeHandle wasm_functype_params(NativeHandle functionType);
            
            [DllImport(NativePlugin.LibraryName)]
            public static extern ValueTypeVector.NativeHandle wasm_functype_results(NativeHandle functionType);

            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_functype_delete(in IntPtr functionType);
        }
    }
}