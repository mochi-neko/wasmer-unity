using System;
using FluentAssertions;
using Mochineko.WasmerUnity.Wasm.Types;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace Mochineko.WasmerUnity.Wasm.Tests.Types
{
    [TestFixture]
    internal sealed class MemoryTypeTest
    {
        [TestCase(uint.MinValue, uint.MinValue)]
        [TestCase(uint.MaxValue, uint.MinValue)]
        [TestCase(uint.MaxValue, uint.MaxValue)]
        [RequiresPlayMode(false)]
        public void CreateTest(uint max, uint min)
        {
            var limit = new Limits(max, min);
            using var memoryType = MemoryType.New(limit);
            memoryType.Should().NotBeNull();
            memoryType.Limits.Should().Be(limit);
            
            GC.Collect();
        }
    }
}