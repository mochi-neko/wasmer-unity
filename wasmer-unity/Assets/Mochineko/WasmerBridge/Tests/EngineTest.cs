using System;
using FluentAssertions;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace Mochineko.WasmerBridge.Tests
{
    [TestFixture]
    internal sealed class EngineTest
    {
        [Test, RequiresPlayMode(false)]
        public void CreateEngineTest()
        {
            using var engine = Engine.New();
            engine.Should().NotBeNull();

            GC.Collect();
        }

        [Test, RequiresPlayMode(false)]
        public void CreateEngineWithConfigTest()
        {
            using var config = Config.New();

            using var engine = Engine.New(config);
            config.Handle.IsClosed.Should().BeTrue();
            engine.Should().NotBeNull();

            GC.Collect();
        }
    }
}