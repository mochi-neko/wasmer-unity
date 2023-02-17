using System;
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
            using var engine = Engine.New();
            using var store = Store.New(engine);
            var binary = MockResource.EmptyWasmBinary;

            Module.Validate(store, binary)
                .Should().BeTrue();

            // Break
            binary[1] = 0x00;

            Module.Validate(store, binary)
                .Should().BeFalse();
        }

        [Test, RequiresPlayMode(false)]
        public void CompileWatTest()
        {
            using var engine = Engine.New();
            using var store = Store.New(engine);
            using var wasm = MockResource.EmptyWat.FromWatToWasm();

            using var module = Module.New(store, "empty", in wasm);
            module.Should().NotBeNull();
        }

        [Test, RequiresPlayMode(false)]
        public void CompileWasmTest()
        {
            using var engine = Engine.New();
            using var store = Store.New(engine);
            using var wasm = ByteVector.New(MockResource.EmptyWasmBinary);

            using var module = Module.New(store, "empty", in wasm);
            module.Should().NotBeNull();
        }
    }
}