using FluentAssertions;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace Mochineko.WasmerBridge.Tests
{
    [TestFixture]
    internal sealed class ConfigTest
    {
        [Test, RequiresPlayMode(false)]
        public void CreateConfigTest()
        {
            using var config = Config.New();
            config.Should().NotBeNull();
        }
    }
}