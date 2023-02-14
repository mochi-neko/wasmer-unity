using FluentAssertions;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace Mochineko.WasmerBridge.Tests
{
    [TestFixture]
    internal sealed class ValueTypeTest
    {
        [RequiresPlayMode(false)]
        [TestCase(ValueKind.Int32)]
        [TestCase(ValueKind.Int64)]
        [TestCase(ValueKind.Float32)]
        [TestCase(ValueKind.Float64)]
        [TestCase(ValueKind.AnyRef)]
        [TestCase(ValueKind.FuncRef)]
        public void CreateValueTypeTest(ValueKind valueKind)
        {
            using var valueType = new ValueType(valueKind);
            valueType.Should().NotBeNull();
            valueType.ValueKind.Should().Be(valueKind);
        }
    }
}