using FluentAssertions;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace Mochineko.WasmerBridge.Tests
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
            var (valueType, nativeHandle) = ValueType.New(valueKind);
            valueType.valueKind.Should().Be(valueKind);
            
            nativeHandle.Dispose();
        }
    }
}