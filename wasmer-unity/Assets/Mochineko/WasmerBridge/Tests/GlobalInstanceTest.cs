using System;
using FluentAssertions;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace Mochineko.WasmerBridge.Tests
{
    [TestFixture]
    internal sealed class GlobalInstanceTest
    {
        [TestCase(ValueKind.Int32, Mutability.Constant, 1)]
        [TestCase(ValueKind.Int64, Mutability.Constant, 2L)]
        [TestCase(ValueKind.Float32, Mutability.Constant, 1.1f)]
        [TestCase(ValueKind.Float64, Mutability.Constant, 1.1d)]
        [RequiresPlayMode(false)]
        public void CreateInstanceTest(ValueKind kind, Mutability mutability, object rawValue)
        {
            using var engine = Engine.New();
            using var store = Store.New(engine);
            using var valueType = ValueType.New(kind);
            using var globalType = GlobalType.New(valueType, mutability);
            var value = ValueInstance.New(kind, rawValue);

            using var globalInstance = GlobalInstance.New(store, globalType, in value);
            globalInstance.Should().NotBeNull();
            globalInstance.Get(out var excludedValue);
            excludedValue.Kind.Should().Be(kind);
            excludedValue.Of.Should().Be(rawValue);

            using var excludedType = globalInstance.Type;
            excludedType.Content.Kind.Should().Be(kind);
            excludedType.Mutability.Should().Be(mutability);

            GC.Collect();
        }

        [TestCase(ValueKind.Int32, 1, 3)]
        [TestCase(ValueKind.Int64, 2L, -1L)]
        [TestCase(ValueKind.Float32, 1.1f, 2.3f)]
        [TestCase(ValueKind.Float64, 1.1d, -2.1d)]
        [RequiresPlayMode(false)]
        public void CreateInstanceAndSetValueTest(ValueKind kind, object rawValue, object changedValue)
        {
            using var engine = Engine.New();
            using var store = Store.New(engine);
            using var valueType = ValueType.New(kind);
            using var globalType = GlobalType.New(valueType, Mutability.Variable);
            var value = ValueInstance.New(kind, rawValue);

            using var globalInstance = GlobalInstance.New(store, globalType, in value);
            globalInstance.Get(out var excludedValue);
            excludedValue.Of.Should().Be(rawValue);

            var changed = ValueInstance.New(kind, changedValue);
            globalInstance.Set(in changed);

            globalInstance.Get(out var excludedChangedValue);
            excludedChangedValue.Of.Should().Be(changedValue);

            GC.Collect();
        }

        [Test]
        [RequiresPlayMode(false)]
        public void CreateInstanceForAnyReferenceTest()
        {
            Action action = () =>
                CreateInstanceTest(ValueKind.AnyRef, Mutability.Constant, (IntPtr)(int.MaxValue));

            action.Should()
                .Throw<NotImplementedException>(because: "Native Wasm API does not implement reference type.");
        }

        [Test]
        [RequiresPlayMode(false)]
        public void CreateInstanceForFunctionReferenceTest()
        {
            Action action = () =>
                CreateInstanceTest(ValueKind.FuncRef, Mutability.Constant, (IntPtr)(int.MaxValue));

            action.Should()
                .Throw<NotImplementedException>(because: "Native Wasm API does not implement reference type.");
        }
    }
}