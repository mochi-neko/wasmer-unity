using System;
using Mochineko.WasmerUnity.Wasm;
using FluentAssertions;
using NUnit.Framework;
using UnityEngine.TestTools;
using ValueType = Mochineko.WasmerUnity.Wasm.ValueType;

namespace Mochineko.WasmerUnity.Wasm.Tests
{
    [TestFixture]
    internal sealed class ExternalTypeTest
    {
        [Test]
        [RequiresPlayMode(false)]
        public void CreateAsFunctionTypeTest()
        {
            using var functionType = FunctionType.New(
                Array.Empty<ValueKind>(),
                Array.Empty<ValueKind>());

            using var externalType = ExternalType.FromFunction(functionType);
            externalType.Should().NotBeNull();
            externalType.Kind.Should().Be(ExternalKind.Function);

            using var excludedFunctionType = externalType.ToFunction();
            excludedFunctionType.Should().NotBeNull();
            excludedFunctionType.Parameters.Length.Should().Be(0);
            excludedFunctionType.Results.Length.Should().Be(0);

            GC.Collect();
        }

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
        public void CreateAsGlobalTypeTest(ValueKind kind, Mutability mutability)
        {
            using var valueType = ValueType.New(kind);
            using var globalType = GlobalType.New(valueType, mutability);

            using var externalType = ExternalType.FromGlobal(globalType);
            externalType.Should().NotBeNull();
            externalType.Kind.Should().Be(ExternalKind.Global);

            using var excludedFunctionType = externalType.ToGlobal();
            excludedFunctionType.Should().NotBeNull();
            excludedFunctionType.Content.Kind.Should().Be(kind);
            excludedFunctionType.Mutability.Should().Be(mutability);

            GC.Collect();
        }

        [TestCase(uint.MinValue, uint.MinValue)]
        [TestCase(uint.MaxValue, uint.MinValue)]
        [TestCase(uint.MaxValue, uint.MaxValue)]
        [RequiresPlayMode(false)]
        public void CreateAsMemoryTypeTest(uint max, uint min)
        {
            var limits = new Limits(max, min);
            using var memoryType = MemoryType.New(in limits);

            using var externalType = ExternalType.FromMemory(memoryType);
            externalType.Should().NotBeNull();
            externalType.Kind.Should().Be(ExternalKind.Memory);

            using var excludedMemoryType = externalType.ToMemory();
            excludedMemoryType.Should().NotBeNull();
            excludedMemoryType.Limits.Should().Be(limits);

            GC.Collect();
        }
    }
}