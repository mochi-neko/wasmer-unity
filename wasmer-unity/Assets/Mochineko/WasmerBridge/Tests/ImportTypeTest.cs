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
        public void CreateImportTypeByEmptyFunctionTest()
        {
            var moduleName = "ModuleName";
            var functionName = "FunctionName";
            using var functionType = FunctionType.New(
                Array.Empty<ValueKind>(),
                Array.Empty<ValueKind>());

            using var importType = ImportType.New(moduleName, functionName, functionType);
            importType.Should().NotBeNull();
            importType.Module.Should().Be(moduleName);
            importType.Name.Should().Be(functionName);
            importType.Kind.Should().Be(ExternalKind.Function);

            GC.Collect();
        }
    }
}