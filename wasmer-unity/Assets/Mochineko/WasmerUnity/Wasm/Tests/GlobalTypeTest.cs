using System;
using Mochineko.WasmerUnity.Wasm;
using FluentAssertions;
using NUnit.Framework;
using UnityEngine.TestTools;
using ValueType = Mochineko.WasmerUnity.Wasm.ValueType;

namespace Mochineko.WasmerUnity.Wasm.Tests
{
    [TestFixture]
    internal sealed class GlobalTypeTest
    {
        [TestCase(ValueKind.Int32, Mutability.Constant)]
        [TestCase(ValueKind.Int32, Mutability.Variable)]
        [TestCase(ValueKind.Int64, Mutability.Constant)]
        [TestCase(ValueKind.Int64, Mutability.Variable)]
        [TestCase(ValueKind.Float32, Mutability.Constant)]
        [TestCase(ValueKind.Float32, Mutability.Variable)]
        [TestCase(ValueKind.Float64, Mutability.Constant)]
        [TestCase(ValueKind.Float64, Mutability.Variable)]
        [TestCase(ValueKind.AnyRef, Mutability.Constant)]
        [TestCase(ValueKind.AnyRef, Mutability.Variable)]
        [TestCase(ValueKind.FuncRef, Mutability.Constant)]
        [TestCase(ValueKind.FuncRef, Mutability.Variable)]
        [RequiresPlayMode(false)]
        public void CreateTest(ValueKind kind, Mutability mutability)
        {
            using var valueType = ValueType.FromKind(kind);

            using var globalType = GlobalType.New(valueType, mutability);
            valueType.Handle.IsClosed.Should().BeTrue();

            globalType.Should().NotBeNull();
            globalType.Content.Kind.Should().Be(kind);
            globalType.Mutability.Should().Be(mutability);

            GC.Collect();
        }
    }
}