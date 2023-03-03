using System;
using FluentAssertions;
using NUnit.Framework;
using UnityEngine.TestTools;
using ValueType = Mochineko.WasmerUnity.Wasm.Types.ValueType;

namespace Mochineko.WasmerUnity.Wasm.Tests.Types
{
    [TestFixture]
    internal sealed class ValueTypeTest
    {
        [TestCase(ValueKind.Int32)]
        [TestCase(ValueKind.Int64)]
        [TestCase(ValueKind.Float32)]
        [TestCase(ValueKind.Float64)]
        [TestCase(ValueKind.AnyRef)]
        [TestCase(ValueKind.FuncRef)]
        [RequiresPlayMode(false)]
        public void CreateValueTypeTest(ValueKind valueKind)
        {
            using var valueType = ValueType.FromKind(valueKind);
            valueType.Should().NotBeNull();
            valueType.Kind.Should().Be(valueKind);
            
            GC.Collect();
        }
    }
}