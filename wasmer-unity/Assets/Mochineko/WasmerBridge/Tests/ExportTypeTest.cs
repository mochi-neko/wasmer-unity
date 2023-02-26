using System;
using FluentAssertions;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace Mochineko.WasmerBridge.Tests
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
            excludedGlobalType.Content.Kind.Should().Be(kind);
            excludedGlobalType.Mutability.Should().Be(mutability);

            GC.Collect();
        }
    }
}