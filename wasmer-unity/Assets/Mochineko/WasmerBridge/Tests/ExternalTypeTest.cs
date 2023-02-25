using System;
using FluentAssertions;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace Mochineko.WasmerBridge.Tests
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
    }
}