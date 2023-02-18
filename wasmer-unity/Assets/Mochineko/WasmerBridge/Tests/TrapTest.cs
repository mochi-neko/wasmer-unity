using FluentAssertions;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace Mochineko.WasmerBridge.Tests
{
    [TestFixture]
    internal sealed class TrapTest
    {
        [Test, RequiresPlayMode(false)]
        public void CreateTrapTest()
        {
            using var engine = Engine.New();
            using var store = Store.New(engine);
            var message = "message";

            using var trap = Trap.New(store, message);
            trap.Should().NotBeNull();

            var excludedMessage = trap.Message;
            excludedMessage.Should().Be(message);
        }
    }
}