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
        }

        [Test, RequiresPlayMode(false)]
        public void CreateEngineWithConfigTest()
        {
            var config = Config.New();
            using var engine = Engine.New(config);
            engine.Should().NotBeNull();
        }
    }
}