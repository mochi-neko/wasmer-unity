using System;
using Mochineko.WasmerUnity.Wasm;
using FluentAssertions;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace Mochineko.WasmerUnity.Wasm.Tests
{
    [TestFixture]
    internal sealed class ConfigTest
    {
        [Test, RequiresPlayMode(false)]
        public void CreateConfigTest()
        {
            using var config = Config.New();
            config.Should().NotBeNull();
            
            GC.Collect();
        }
    }
}