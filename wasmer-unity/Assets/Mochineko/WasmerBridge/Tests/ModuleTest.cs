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
            
            GC.Collect();
        }

        [Test, RequiresPlayMode(false)]
        public void CompileWatTest()
        {
            using var engine = Engine.New();
            using var store = Store.New(engine);
            MockResource.EmptyWat.FromWatToWasm(out var wasm);
            using (wasm)
            {
                using var module = Module.New(store, "empty", in wasm);
                module.Should().NotBeNull();
            }
            
            GC.Collect();
        }

        [Test, RequiresPlayMode(false)]
        public void CompileWasmTest()
        {
            using var engine = Engine.New();
            using var store = Store.New(engine);
            ByteVector.New(MockResource.EmptyWasmBinary, out var wasm);
            using (wasm)
            {
                using var module = Module.New(store, "empty", in wasm);
                module.Should().NotBeNull();
            }
            
            GC.Collect();
        }

        [Test, RequiresPlayMode(false)]
        public void SerializeTest()
        {
            using var engine = Engine.New();
            using var store = Store.New(engine);
            ByteVector.New(MockResource.EmptyWasmBinary, out var wasm);
            using (wasm)
            {
                using var module = Module.New(store, "empty", in wasm);

                module.Serialize(out var serialized);
                serialized.Length.Should().NotBe(0);

                var deserialized = Module.Deserialize(store, "empty", serialized);
                deserialized.Should().NotBeNull();
            }
            
            GC.Collect();
        }
    }
}