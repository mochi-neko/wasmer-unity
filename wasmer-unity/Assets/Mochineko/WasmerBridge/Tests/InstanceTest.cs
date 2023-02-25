using System;
using FluentAssertions;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace Mochineko.WasmerBridge.Tests
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

        [Test, RequiresPlayMode(false)]
        [Ignore("Implementing")]
        public void InstantiateModuleWithFunctionTest()
        {
            // Imports -> imported_function
            // Exports -> exported_function
            const string wat = @"(module
  (type $t0 (func))
  (import """" ""imported_function"" (func $.imported_function (type $t0)))
  (func $exported_function
    call $.imported_function
  )
  (export ""exported_function"" (func $exported_function))
)";

            using var engine = Engine.New();
            using var store = Store.New(engine);
            wat.ToWasm(out var wasm);
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