using System;
using Mochineko.WasmerUnity.Wasm;
using FluentAssertions;
using NUnit.Framework;
using UnityEngine.TestTools;
using ValueType = Mochineko.WasmerUnity.Wasm.ValueType;

namespace Mochineko.WasmerUnity.Wasm.Tests
{
    [TestFixture]
    internal sealed class ExportTypeTest
    {
        [Test]
        [RequiresPlayMode(false)]
        public void CreateByEmptyFunctionTest()
        {
            var name = "FunctionName";
            using var functionType = FunctionType.New(
                Array.Empty<ValueKind>(),
                Array.Empty<ValueKind>());

            using var exportType = ExportType.FromFunction(name, functionType);
            functionType.Handle.IsClosed.Should().BeTrue();

            exportType.Should().NotBeNull();
            exportType.Name.Should().Be(name);
            exportType.Kind.Should().Be(ExternalKind.Function);

            using var excludedFunctionType = exportType.Type.ToFunction();
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
        public void CreateFromGlobalTest(ValueKind kind, Mutability mutability)
        {
            var name = "GlobalName";
            using var valueType = ValueType.New(kind);
            using var globalType = GlobalType.New(valueType, mutability);

            using var exportType = ExportType.FromGlobal(name, globalType);
            globalType.Handle.IsClosed.Should().BeTrue();

            exportType.Should().NotBeNull();
            exportType.Name.Should().Be(name);
            exportType.Kind.Should().Be(ExternalKind.Global);

            using var excludedGlobalType = exportType.Type.ToGlobal();
            excludedGlobalType.Should().NotBeNull();
            excludedGlobalType.Content.Kind.Should().Be(kind);
            excludedGlobalType.Mutability.Should().Be(mutability);

            GC.Collect();
        }

        [TestCase(uint.MinValue, uint.MinValue)]
        [TestCase(uint.MaxValue, uint.MinValue)]
        [TestCase(uint.MaxValue, uint.MaxValue)]
        [RequiresPlayMode(false)]
        public void CreateFromMemoryTest(uint max, uint min)
        {
            var name = "MemoryName";
            var limits = new Limits(max, min);
            using var memoryType = MemoryType.New(limits);

            using var exportType = ExportType.FromMemory(name, memoryType);
            memoryType.Handle.IsClosed.Should().BeTrue();

            exportType.Should().NotBeNull();
            exportType.Name.Should().Be(name);
            exportType.Kind.Should().Be(ExternalKind.Memory);

            using var excludedGlobalType = exportType.Type.ToMemory();
            excludedGlobalType.Should().NotBeNull();
            excludedGlobalType.Limits.Should().Be(limits);

            GC.Collect();
        }
    }
}