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
        public void CompileEmptyWasmTest()
        {
            using var engine = Engine.New();
            using var store = Store.New(engine);
            ByteVector.New(MockResource.EmptyWasmBinary, out var wasm);
            using (wasm)
            {
                using var module = Module.New(store, in wasm);
                module.Should().NotBeNull();
            }
            
            GC.Collect();
        }

        [Test, RequiresPlayMode(false)]
        public void CompileEmptyWatTest()
        {
            using var engine = Engine.New();
            using var store = Store.New(engine);
            MockResource.EmptyWat.ToWasm(out var wasm);
            using (wasm)
            {
                using var module = Module.New(store, in wasm);
                module.Should().NotBeNull();
                
                module.Imports(out var imports);
                using (imports)
                {
                    imports.size.Should().Be((nuint)0);
                }
                
                module.Exports(out var exports);
                using (exports)
                {
                    exports.size.Should().Be((nuint)0);
                }
            }
            
            GC.Collect();
        }

        [Test, RequiresPlayMode(false)]
        public void SerializeEmptyWasmTest()
        {
            using var engine = Engine.New();
            using var store = Store.New(engine);
            ByteVector.New(MockResource.EmptyWasmBinary, out var wasm);
            using (wasm)
            {
                using var module = Module.New(store, in wasm);

                module.Serialize(out var serialized);
                serialized.Length.Should().NotBe(0);

                var deserialized = Module.Deserialize(store, serialized);
                deserialized.Should().NotBeNull();
            }
            
            GC.Collect();
        }
        
        [Test, RequiresPlayMode(false)]
        public void CompileHelloWorldWatTest()
        {
            using var engine = Engine.New();
            using var store = Store.New(engine);
            MockResource.HelloWorldWat.ToWasm(out var wasm);
            using (wasm)
            {
                using var module = Module.New(store, in wasm);
                module.Should().NotBeNull();
                
                module.Imports(out var imports);
                using (imports)
                {
                    imports.size.Should().Be((nuint)1);
                }
                
                module.Exports(out var exports);
                using (exports)
                {
                    exports.size.Should().Be((nuint)1);
                }
            }
            
            GC.Collect();
        }
    }
}