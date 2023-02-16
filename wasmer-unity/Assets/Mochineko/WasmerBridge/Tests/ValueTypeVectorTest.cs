using System;
using FluentAssertions;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace Mochineko.WasmerBridge.Tests
{
    [TestFixture]
    internal sealed class ValueTypeVectorTest
    {
        [Test, RequiresPlayMode(false)]
        public void CreateEmptyTest()
        {
            using var vector = ValueTypeVector.New();
            vector.size.Should().Be((nuint)0);
            
            using var emptyVector = ValueTypeVector.New(ArraySegment<ValueKind>.Empty);
            emptyVector.size.Should().Be((nuint)0);
        }

        [Test, RequiresPlayMode(false)]
        public void CreateFromManagedArrayTest()
        {
            var valueKinds = new[]
            {
                ValueKind.Int32,
                ValueKind.Int64,
                ValueKind.Float32,
                ValueKind.Float64,
                ValueKind.AnyRef,
                ValueKind.FuncRef,
            };

            using var vector = ValueTypeVector.New(valueKinds);
            vector.size.Should().Be((nuint)valueKinds.Length);

            var kinds = ValueTypeVector.ToValueKinds(vector);
            kinds[0].Should().Be(ValueKind.Int32);
            kinds[1].Should().Be(ValueKind.Int64);
            kinds[2].Should().Be(ValueKind.Float32);
            kinds[3].Should().Be(ValueKind.Float64);
            kinds[4].Should().Be(ValueKind.AnyRef);
            kinds[5].Should().Be(ValueKind.FuncRef);
        }
    }
}