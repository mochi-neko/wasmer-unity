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

            using var excludedType = exportType.Type.ToFunction();
            excludedType.Should().NotBeNull();
            excludedType.Parameters.Length.Should().Be(0);
            excludedType.Results.Length.Should().Be(0);

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
            using var valueType = ValueType.FromKind(kind);
            using var globalType = GlobalType.New(valueType, mutability);

            using var exportType = ExportType.FromGlobal(name, globalType);
            globalType.Handle.IsClosed.Should().BeTrue();

            exportType.Should().NotBeNull();
            exportType.Name.Should().Be(name);
            exportType.Kind.Should().Be(ExternalKind.Global);

            using var excludedType = exportType.Type.ToGlobal();
            excludedType.Should().NotBeNull();
            excludedType.Content.Kind.Should().Be(kind);
            excludedType.Mutability.Should().Be(mutability);

            GC.Collect();
        }
        
        [TestCase(ValueKind.Int32, uint.MinValue, uint.MinValue)]
        [TestCase(ValueKind.Float32, uint.MaxValue, uint.MinValue)]
        [TestCase(ValueKind.Int64, uint.MaxValue, uint.MaxValue)]
        [RequiresPlayMode(false)]
        public void CreateFromTableTest(ValueKind kind, uint max, uint min)
        {
            var name = "TableName";
            var limits = new Limits(max, min);
            using var element = ValueType.FromKind(kind);
            using var tableType = TableType.New(element, in limits);
            
            using var importType = ExportType.FromTable(name, tableType);
            tableType.Handle.IsClosed.Should().BeTrue();

            importType.Should().NotBeNull();
            importType.Name.Should().Be(name);
            importType.Kind.Should().Be(ExternalKind.Table);

            using var excludedType = importType.Type.ToTable();
            excludedType.Should().NotBeNull();
            excludedType.Element.Kind.Should().Be(kind);
            excludedType.Limits.Should().Be(limits);

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

            using var excludedType = exportType.Type.ToMemory();
            excludedType.Should().NotBeNull();
            excludedType.Limits.Should().Be(limits);

            GC.Collect();
        }
    }
}