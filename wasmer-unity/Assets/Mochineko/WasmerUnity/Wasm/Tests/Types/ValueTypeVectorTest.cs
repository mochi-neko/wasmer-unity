using System;
using FluentAssertions;
using Mochineko.WasmerUnity.Wasm.Types;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace Mochineko.WasmerUnity.Wasm.Tests.Types
{
    [TestFixture]
    internal sealed class ValueTypeVectorTest
    {
        [Test, RequiresPlayMode(false)]
        public void CreateEmptyTest()
        {
            ValueTypeVector.NewEmpty(out var vector);
            using (vector)
            {
                vector.size.Should().Be((nuint)0);
            }

            ValueTypeVector.New(ArraySegment<ValueKind>.Empty, out var emptyVector);
            using (emptyVector)
            {
                emptyVector.size.Should().Be((nuint)0);   
            }
            
            GC.Collect();
        }

        [Test, RequiresPlayMode(false)]
        public void CreateFromManagedArrayTest()
        {
            var kinds = new[]
            {
                ValueKind.Int32,
                ValueKind.Int64,
                ValueKind.Float32,
                ValueKind.Float64,
                ValueKind.AnyRef,
                ValueKind.FuncRef,
            };

            ValueTypeVector.New(kinds, out var vector);
            using (vector)
            {
                vector.size.Should().Be((nuint)kinds.Length);

                vector.ToKinds(out var excludedKinds);
                excludedKinds[0].Should().Be(ValueKind.Int32);
                excludedKinds[1].Should().Be(ValueKind.Int64);
                excludedKinds[2].Should().Be(ValueKind.Float32);
                excludedKinds[3].Should().Be(ValueKind.Float64);
                excludedKinds[4].Should().Be(ValueKind.AnyRef);
                excludedKinds[5].Should().Be(ValueKind.FuncRef);
            }
            
            GC.Collect();
        }
    }
}