using System;
using FluentAssertions;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace Mochineko.WasmerBridge.Tests
{
    [TestFixture]
    internal sealed class FunctionTypeTest
    {
        [TestCase(new ValueKind[] { }, new ValueKind[] { })]
        [TestCase(new[] { ValueKind.Int32 }, new ValueKind[] { })]
        [TestCase(new[] { ValueKind.Int32, ValueKind.Int32 }, new ValueKind[] { })]
        [TestCase(new ValueKind[] { }, new[] { ValueKind.Int32 })]
        [TestCase(
            new[]
            {
                ValueKind.Int32, ValueKind.Int64, ValueKind.Float32, 
                ValueKind.Float64, ValueKind.AnyRef, ValueKind.FuncRef
            },
            new[]
            {
                ValueKind.Int32, ValueKind.Int64, ValueKind.Float32,
                ValueKind.Float64, ValueKind.AnyRef, ValueKind.FuncRef
            })]
        [RequiresPlayMode(false)]
        public void CreateFunctionTypeWithParametersAndResultsTest(ValueKind[] parameterKinds, ValueKind[] resultsKind)
        {
            using var functionType = FunctionType.New(
                parameterKinds,
                resultsKind);
            functionType.Should().NotBeNull();

            functionType.Parameters.Length.Should().Be(parameterKinds.Length);
            functionType.Results.Length.Should().Be(resultsKind.Length);
            
            GC.Collect();
        }
    }
}