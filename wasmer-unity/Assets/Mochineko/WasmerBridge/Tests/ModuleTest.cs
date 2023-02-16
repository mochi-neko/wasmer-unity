using FluentAssertions;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace Mochineko.WasmerBridge.Tests
{
    [TestFixture]
    internal sealed class ModuleTest
    {
        [Test, RequiresPlayMode(false)]
        public void ValidateWasmTest()
        {
            using var engine = new Engine();
            using var store = new Store(engine);
            using var wasm = ByteVector.New(MockResource.EmptyWasmBinary);

            Module.Validate(store, in wasm).Should().BeTrue();

            using var notWasm = ByteVector.New(new byte[8]
                {
                    0x00, 0x00, 0x00, 0x00,
                    0x00, 0x00, 0x00, 0x00,
                }
            );

            Module.Validate(store, in notWasm).Should().BeFalse();
        }

        [Test, RequiresPlayMode(false)]
        [Ignore("Remains crashes")]
        public void CompileWatTest()
        {
            using var engine = new Engine();
            using var store = new Store(engine);
            using var wasm = MockResource.EmptyWat.ToWasm();

            using var module = new Module(store, "empty", in wasm);
            module.Should().NotBeNull();
        }

        [Test, RequiresPlayMode(false)]
        [Ignore("Remains crashes")]
        public void CompileWasmTest()
        {
            using var engine = new Engine();
            using var store = new Store(engine);
            using var wasm = ByteVector.New(MockResource.EmptyWasmBinary);

            using var module = new Module(store, "empty", in wasm);
            module.Should().NotBeNull();
        }
    }
}