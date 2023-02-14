using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace Mochineko.WasmerBridge.Tests
{
    [TestFixture]
    internal sealed class ValueTypeArrayTest
    {
        [Test, RequiresPlayMode(false)]
        public void CreateEmptyTest()
        {
            using var array = ValueTypeArray.New();
            array.size.Should().Be((nuint)0);
        }

        [Test, RequiresPlayMode(false)]
        public void CreateFromManagedTypeArrayTest()
        {
            var valueTypeArray = new[]
                {
                    ValueType.New(ValueKind.Int32),
                    ValueType.New(ValueKind.Int64),
                    ValueType.New(ValueKind.Float32),
                    ValueType.New(ValueKind.Float64),
                }
                .Select(tuple => tuple.handle)
                .ToArray();

            var array = ValueTypeArray.New(valueTypeArray);
            array.size.Should().Be((nuint)valueTypeArray.Length);

            foreach (var valueType in valueTypeArray)
            {
                valueType.Dispose();
            }
        }
    }
}