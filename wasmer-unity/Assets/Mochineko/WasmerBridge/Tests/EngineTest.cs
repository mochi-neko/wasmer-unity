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
            using var engine = new Engine();
            engine.Should().NotBeNull();
        }

        [Test, RequiresPlayMode(false)]
        [Ignore("Remains crashes")]
        public void CreateEngineWithConfigTest()
        {
            using var config = new Config();
            using var engine = new Engine(config);
            engine.Should().NotBeNull();
        }
    }
}