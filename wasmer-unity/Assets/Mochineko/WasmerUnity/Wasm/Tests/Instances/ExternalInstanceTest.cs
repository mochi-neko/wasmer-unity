using System;
using FluentAssertions;
using Mochineko.WasmerUnity.Wasm.Instances;
using Mochineko.WasmerUnity.Wasm.Types;
using NUnit.Framework;
using UnityEngine.TestTools;
using ValueType = Mochineko.WasmerUnity.Wasm.Types.ValueType;

namespace Mochineko.WasmerUnity.Wasm.Tests.Instances
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

            using var externalInstance = ExternalInstance.FromFunctionWithOwnership(functionInstance);
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
                using var trap = excluded.Call(in arguments, ref results);
                trap.Should().BeNull();
                callbackCalled.Should().BeTrue();
            }

            GC.Collect();
        }
        
        [Test, RequiresPlayMode(false)]
        public unsafe void CreateInstanceFromGlobalTest()
        {
            using var engine = Engine.New();
            using var store = Store.New(engine);
            using var valueType = ValueType.FromKind(ValueKind.Int32);
            using var globalType = GlobalType.New(valueType, Mutability.Variable);
            var value = ValueInstance.NewInt32(1);
            using var globalInstance = GlobalInstance.New(store, globalType, in value);
            
            using var externalInstance = ExternalInstance.FromGlobalWithOwnership(globalInstance);
            externalInstance.Should().NotBeNull();
            externalInstance.Kind.Should().Be(ExternalKind.Global);
            using var type = externalInstance.Type;
            type.Handle.IsInvalid.Should().BeFalse();
            type.Kind.Should().Be(ExternalKind.Global);

            using var excluded = externalInstance.ToGlobal();
            excluded.Should().NotBeNull();
            var excludedValue = excluded.Get();
            excludedValue.Kind.Should().Be(ValueKind.Int32);
            excludedValue.OfInt32.Should().Be(1);

            GC.Collect();
        }
    }
}