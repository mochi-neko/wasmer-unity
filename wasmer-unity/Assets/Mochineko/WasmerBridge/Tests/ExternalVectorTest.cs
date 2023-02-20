using System;
using FluentAssertions;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace Mochineko.WasmerBridge.Tests
{
    [TestFixture]
    internal sealed class ExternalVectorTest
    {
        [Test, RequiresPlayMode(false)]
        [Ignore("Not implemented")]
        public void CreateEmptyTest()
        {
            ExternalVector.NewEmpty(out var vector);
            using (vector)
            {
                vector.size.Should().Be((nuint)0);
            }

            ExternalVector.New(ArraySegment<ExternalKind>.Empty, out var emptyVector);
            using (emptyVector)
            {
                emptyVector.size.Should().Be((nuint)0);
            }
            
            GC.Collect();
        }

        [Test, RequiresPlayMode(false)]
        [Ignore("Not implemented")]
        public void CreateFromManagedArrayTest()
        {
            var kinds = new[]
            {
                ExternalKind.Function,
                ExternalKind.Global,
                ExternalKind.Table,
                ExternalKind.Memory,
            };

            ExternalVector.New(kinds, out var vector);
            using (vector)
            {
                vector.size.Should().Be((nuint)kinds.Length);

                vector.ToKinds(out var excludedKinds);

                excludedKinds[0].Should().Be(ExternalKind.Function);
                excludedKinds[1].Should().Be(ExternalKind.Global);
                excludedKinds[2].Should().Be(ExternalKind.Table);
                excludedKinds[3].Should().Be(ExternalKind.Memory);
            }
            
            GC.Collect();
        }
    }
}