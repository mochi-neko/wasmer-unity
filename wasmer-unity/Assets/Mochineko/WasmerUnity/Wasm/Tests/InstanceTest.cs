using System;
using Mochineko.WasmerUnity.Wasm;
using FluentAssertions;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace Mochineko.WasmerUnity.Wasm.Tests
{
    [TestFixture]
    internal sealed class InstanceTest
    {
        [Test, RequiresPlayMode(false)]
        public void InstantiateEmptyModuleTest()
        {
            using var engine = Engine.New();
            using var store = Store.New(engine);
            ByteVector.New(MockResource.EmptyWasmBinary, out var wasm);
            using (wasm)
            {
                using var module = Module.New(store, in wasm);

                ExternalInstanceVector.NewEmpty(out var imports);
                using (imports)
                {
                    using var instance = Instance.New(store, module, in imports);
                    instance.Should().NotBeNull();
                    instance.Exports(out var exports);
                    using (exports)
                    {
                        exports.size.Should().Be((nuint)0);
                    }
                }
            }

            GC.Collect();
        }
    }
}