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

            using var externalType = ExternalType.ToExternalType(functionType);
            externalType.Should().NotBeNull();
            externalType.Kind.Should().Be(ExternalKind.Function);

            using var excludedFunctionType = externalType.ToFunctionType();
            excludedFunctionType.Should().NotBeNull();
            excludedFunctionType.Parameters.Length.Should().Be(0);
            excludedFunctionType.Results.Length.Should().Be(0);
            
            // Original FunctionType has ownerships of ExternalType and excluded FunctionType from ExternalType.
            externalType.Handle.SetHandleAsInvalid();
            excludedFunctionType.Handle.SetHandleAsInvalid();
            
            GC.Collect();
        }
    }
}