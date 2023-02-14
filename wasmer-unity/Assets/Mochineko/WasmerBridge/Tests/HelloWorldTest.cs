using FluentAssertions;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace Mochineko.WasmerBridge.Tests
{
    [TestFixture]
    internal sealed class HelloWorldTest
    {
        [Test, RequiresPlayMode(false)]
        [Ignore("Not Implemented")]
        public void InstantiateModuleTest()
        {
            using var engine = new Engine();
            using var store = new Store(engine);
            using var wasm = NativeByteArray.CreateFromManaged(MockModule.EmptyWasmBinary);
            using var module = new Module(store, "empty", wasm);
            //using var importObject = new ImportObject();

            //var instance = new Instantiate(store, module, inportObject);
            //instance.Should().NotBeNull();
        }

        [Test, RequiresPlayMode(false)]
        [Ignore("Not Implemented")]
        public void HelloWorld()
        {
            // WebAssembly Text Format
            const string wat = @"
(module
  (type $t0 (func))
  (import """" ""hello"" (func $.hello (type $t0)))
  (func $run
    call $.hello
  )
  (export ""run"" (func $run))
)";

            // Specify engine.
            using var engine = new Engine();

            // Create store.
            using var store = new Store(engine);

            // Compile wasm.
            using var module = new Module(store, "hello", wat.ToWasm());

            // Define "hello" function as import object.
            var importObject = new ImportObject();
            bool helloCalled = false;
            importObject.AddFunction("Mochineko.WasmerBridge.Tests", "hello", () => helloCalled = true);

            // Instantiate module.
            var instance = new Instance(store, module, importObject);

            // Get exported function.
            var run = instance.ExportFunction<Unit>(store, "run");

            // Call exported function.
            run.Invoke();

            // Assert flag.
            helloCalled.Should().Be(true);
        }
    }

    public sealed class Unit
    {
        private Unit()
        {
        }

        public static readonly Unit Default = new();
    }
}