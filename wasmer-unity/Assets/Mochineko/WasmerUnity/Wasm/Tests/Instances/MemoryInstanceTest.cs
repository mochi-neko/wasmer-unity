using System;
using FluentAssertions;
using Mochineko.WasmerUnity.Wasm.Instances;
using Mochineko.WasmerUnity.Wasm.Types;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace Mochineko.WasmerUnity.Wasm.Tests.Instances
{
    [TestFixture]
    internal sealed class MemoryInstanceTest
    {
        [Test]
        [RequiresPlayMode(false)]
        public unsafe void CreateTest()
        {
            using var engine = Engine.New();
            using var store = Store.New(engine);

            var limits = new Limits(1, 0);
            using var type = MemoryType.New(limits);

            using var instance = MemoryInstance.New(store, type);
            instance.Should().NotBeNull();
            instance.Type.Limits.Should().Be(limits);
            instance.Size.Should().Be(0);
            instance.DataSize.Should().Be((nuint)0);

            instance.Grow(1);
            instance.Size.Should().Be(1);
            instance.DataSize.Should().Be(MemoryInstance.MemoryPageSize);

            *instance.Data = 0x0001;
            (*instance.Data).Should().Be(0x0001);

            GC.Collect();
        }
    }
}