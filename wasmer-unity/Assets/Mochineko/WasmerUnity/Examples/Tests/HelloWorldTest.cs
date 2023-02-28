using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using UnityEngine.TestTools;
using Mochineko.WasmerUnity.Wasm;
using Mochineko.WasmerUnity.Wasmer;

namespace Mochineko.WasmerUnity.Examples.Tests
{
    /// <summary>
    /// Original: https://github.com/wasmerio/wasmer/blob/master/examples/hello_world.rs
    /// </summary>
    [TestFixture]
    internal sealed unsafe class HelloWorldTest
    {
        [Test, RequiresPlayMode(false)]
        public void HelloWorld()
        {
            // First we create a simple Wasm program to use with Wasmer.
            // We use the WebAssembly text format and use `wasmer::wat2wasm` to compile
            // it into a WebAssembly binary.
            //
            // Most WebAssembly programs come from compiling source code in a high level
            // language and will already be in the binary format.
            var wasmBytes = @"
(module
    (type $t0 (func))
    (import ""env"" ""say_hello"" (func $.say_hello (type $t0)))
    (func $run
        call $.say_hello
    )
    (export ""run"" (func $run))
)"
                .WatToWasm();

            // Next we create the `Store`, the top level type in the Wasmer API.
            //
            // Note that we don't need to specify the engine/compiler if we want to use
            // the default provided by Wasmer.
            // You can use `Store::default()` for that.
            //
            // However for the purposes of showing what's happening, we create a compiler
            // (`Cranelift`) and pass it to an engine (`Universal`). We then pass the engine to
            // the store and are now ready to compile and run WebAssembly!
            using var store = Store.New();

            // We then use our store and Wasm bytes to compile a `Module`.
            // A `Module` is a compiled WebAssembly module that isn't ready to execute yet.
            using var module = Module.New(store, wasmBytes);

            // We define a function to act as our "env" "say_hello" function imported in the
            // Wasm program above.
            bool helloCalled = false;

            void SayHelloWorld()
            {
                helloCalled = true;
            }

            // We then create an import object so that the `Module`'s imports can be satisfied.
            using var importObject = ImportObject.New(
                new Dictionary<string, IReadOnlyDictionary<string, ExternalInstance>>
                {
                    ["env"] = new Dictionary<string, ExternalInstance>
                    {
                        ["say_hello"] =
                            ExternalInstance.FromFunctionWithOwnership(FunctionInstance.New(store, SayHelloWorld))
                    }
                });

            // We then use the `Module` and the import object to create an `Instance`.
            //
            // An `Instance` is a compiled WebAssembly module that has been set up
            // and is ready to execute.
            using var instance = Instance.New(store, module, importObject);

            // We get the `TypedFunction` with no parameters and no results from the instance.
            //
            // Recall that the Wasm module exported a function named "run", this is getting
            // that exported function from the `Instance`.
            var run = instance.GetFunction(store, "run");

            // Finally, we call our exported Wasm function which will call our "say_hello"
            // function and return.
            run.Call();

            // Assert flag.
            helloCalled.Should().BeTrue();
        }
    }
}