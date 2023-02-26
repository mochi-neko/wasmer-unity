using System;
using Mochineko.WasmerUnity.Wasm;
using FluentAssertions;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace Mochineko.WasmerUnity.Wasmer.Tests
{
    [TestFixture]
    internal sealed class ModuleTest
    {
        [Test, RequiresPlayMode(false)]
        public void CompileHelloWorldWatTest()
        {
            const string HelloWorldWat = @"
(module
  (type $t0 (func))
  (import """" ""hello"" (func $.hello (type $t0)))
  (func $run
    call $.hello
  )
  (export ""run"" (func $run))
)";

            using var engine = Engine.New();
            using var store = Store.New(engine);
            var wasm = HelloWorldWat.ToWasm();

            using var module = Module.FromBinary(store, wasm);
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

            GC.Collect();
        }
    }
}