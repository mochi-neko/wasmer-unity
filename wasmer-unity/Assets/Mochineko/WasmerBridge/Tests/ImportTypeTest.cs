using System;
using FluentAssertions;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace Mochineko.WasmerBridge.Tests
{
    [TestFixture]
    internal sealed class ImportTypeTest
    {
        [Test]
        [RequiresPlayMode(false)]
        public void CreateFromEmptyFunctionTest()
        {
            var moduleName = "ModuleName";
            var name = "FunctionName";
            using var functionType = FunctionType.New(
                Array.Empty<ValueKind>(),
                Array.Empty<ValueKind>());

            using var importType = ImportType.FromFunction(moduleName, name, functionType);
            functionType.Handle.IsClosed.Should().BeTrue();

            importType.Should().NotBeNull();
            importType.Module.Should().Be(moduleName);
            importType.Name.Should().Be(name);
            importType.Kind.Should().Be(ExternalKind.Function);

            using var excludedFunctionType = importType.Type.ToFunction();
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
            var moduleName = "ModuleName";
            var name = "GlobalName";
            using var valueType = ValueType.New(kind);
            using var globalType = GlobalType.New(valueType, mutability);

            using var importType = ImportType.FromGlobal(moduleName, name, globalType);
            globalType.Handle.IsClosed.Should().BeTrue();

            importType.Should().NotBeNull();
            importType.Module.Should().Be(moduleName);
            importType.Name.Should().Be(name);
            importType.Kind.Should().Be(ExternalKind.Global);

            using var excludedGlobalType = importType.Type.ToGlobal();
            excludedGlobalType.Content.Kind.Should().Be(kind);
            excludedGlobalType.Mutability.Should().Be(mutability);

            GC.Collect();
        }
    }
}