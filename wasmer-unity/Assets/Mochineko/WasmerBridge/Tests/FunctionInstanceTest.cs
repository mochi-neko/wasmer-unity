using System;
using FluentAssertions;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace Mochineko.WasmerBridge.Tests
{
    [TestFixture]
    internal sealed class FunctionInstanceTest
    {
        [Test, RequiresPlayMode(false)]
        public unsafe void CreateFunctionInstanceTest()
        {
            using var engine = Engine.New();
            using var store = Store.New(engine);
            using var functionType = FunctionType.New(
                Array.Empty<ValueKind>(),
                Array.Empty<ValueKind>());

            bool callbackCalled = false;

            using var functionInstance = FunctionInstance.New(
                store,
                functionType,
                callback: delegate(ValueInstanceVector* args, ValueInstanceVector* rets)
                {
                    callbackCalled = true;
                    return IntPtr.Zero;
                });
            functionInstance.Should().NotBeNull();
            functionInstance.Type.Parameters.Length.Should().Be(0);
            functionInstance.Type.Results.Length.Should().Be(0);
            functionInstance.ParametersArity.Should().Be((nuint)0);
            functionInstance.ResultsArity.Should().Be((nuint)0);

            ValueInstanceVector.NewEmpty(out var arguments);
            ValueInstanceVector.NewEmpty(out var results);
            using (arguments)
            using (results)
            {
                using var trap = functionInstance.Call(in arguments, ref results);
                trap.Should().BeNull();
                callbackCalled.Should().BeTrue();
            }

            GC.Collect();
        }

        [Test, RequiresPlayMode(false)]
        [Ignore("Does not implement environment")]
        public unsafe void CreateFunctionInstanceWithEnvironmentTest()
        {
            using var engine = Engine.New();
            using var store = Store.New(engine);
            using var functionType = FunctionType.New(
                Array.Empty<ValueKind>(),
                Array.Empty<ValueKind>());

            bool callbackCalled = false;

            using var functionInstance = FunctionInstance.NewWithEnvironment(
                store,
                functionType,
                callback: delegate(IntPtr environment, ValueInstanceVector* vector, ValueInstanceVector* instanceVector)
                {
                    callbackCalled = true;
                    return IntPtr.Zero;
                },
                environment: IntPtr.Zero,
                finalizer: delegate(IntPtr environment) { }
            );
            functionInstance.Should().NotBeNull();
            functionInstance.Type.Parameters.Length.Should().Be(0);
            functionInstance.Type.Results.Length.Should().Be(0);
            functionInstance.ParametersArity.Should().Be((nuint)0);
            functionInstance.ResultsArity.Should().Be((nuint)0);

            ValueInstanceVector.NewEmpty(out var arguments);
            ValueInstanceVector.NewEmpty(out var results);
            using (arguments)
            using (results)
            {
                using var trap = functionInstance.Call(in arguments, ref results);
                trap.Should().BeNull();
                callbackCalled.Should().BeTrue();
            }

            GC.Collect();
        }
    }
}