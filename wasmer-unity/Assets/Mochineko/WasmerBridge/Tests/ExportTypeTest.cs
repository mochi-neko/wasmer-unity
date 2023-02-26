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
            var functionName = "FunctionName";
            using var functionType = FunctionType.New(
                Array.Empty<ValueKind>(),
                Array.Empty<ValueKind>());

            using var importType = ExportType.New(functionName, functionType);
            functionType.Handle.IsClosed.Should().BeTrue();
            importType.Should().NotBeNull();
            importType.Name.Should().Be(functionName);
            importType.Kind.Should().Be(ExternalKind.Function);

            GC.Collect();
        }
    }
}