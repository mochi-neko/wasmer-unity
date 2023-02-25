using System;
using FluentAssertions;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace Mochineko.WasmerBridge.Tests
{
    [TestFixture]
    internal sealed class ExternalInstanceTest
    {
        [Test, RequiresPlayMode(false)]
        public unsafe void CreateInstanceFromFunctionTest()
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
                callback: (_, _) =>
                {
                    callbackCalled = true;
                    return IntPtr.Zero;
                });

            using var externalInstance = ExternalInstance.FromFunction(functionInstance);
            externalInstance.Should().NotBeNull();
            externalInstance.Kind.Should().Be(ExternalKind.Function);
            using var type = externalInstance.Type;
            type.Handle.IsInvalid.Should().BeFalse();
            type.Kind.Should().Be(ExternalKind.Function);

            using var excluded = externalInstance.ToFunction();
            excluded.Should().NotBeNull();
            excluded.ParametersArity.Should().Be((nuint)0);
            excluded.ResultsArity.Should().Be((nuint)0);

            ValueInstanceVector.NewEmpty(out var arguments);
            ValueInstanceVector.NewEmpty(out var results);
            using (arguments)
            using (results)
            {
                var trap = excluded.Call(in arguments, ref results);
                trap.Should().BeNull();
                callbackCalled.Should().BeTrue();
            }

            GC.Collect();
        }
    }
}