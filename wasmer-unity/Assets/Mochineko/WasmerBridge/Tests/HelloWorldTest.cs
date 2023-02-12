using System;
using FluentAssertions;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace Mochineko.WasmerBridge.Tests
{
    internal sealed class HelloWorldTest
    {
        [Test, RequiresPlayMode(false)]
        public void InitializeEngineTest()
        {
            using var engine = new Engine();
            engine.Should().NotBeNull();
        }
        
        [Test, RequiresPlayMode(false)]
        public void InitializeStoreTest()
        {
            using var engine = new Engine();

            using var store = new Store(engine);
            store.Should().NotBeNull();
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
            using var module = new Module(store, wat);

            // Define "hello" function as import object.
            var importObject = new ImportObject();
            bool helloCalled = false;
            importObject.AddFunction("Mochineko.WasmerBridge.Tests", "hello", () => helloCalled = true);

            // Instantiate module.
            var instance = module.Instantiate(store, module, importObject);

            // Get exported function.
            var run = instance.ExportFunction<Unit>(store, "run");

            // Call exported function.
            run.Invoke();

            // Assert flag.
            helloCalled.Should().Be(true);
        }
    }

    public sealed class Module : IDisposable
    {
        public Module(Store store, string wat)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
        }

        public Instance Instantiate(Store store, Module module, ImportObject importObject)
        {
            throw new NotImplementedException();
        }
    }

    public sealed class ImportObject
    {
        public void AddFunction<T>(string nameSpace, string functionName, Func<T> func)
        {
            throw new NotImplementedException();
        }
    }

    public sealed class Instance
    {
        public Func<T> ExportFunction<T>(Store store, string run)
        {
            throw new NotImplementedException();
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