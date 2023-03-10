using FluentAssertions;
using Mochineko.WasmerUnity.Wasm;
using Mochineko.WasmerUnity.Wasm.Instances;
using Mochineko.WasmerUnity.Wasmer;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace Mochineko.WasmerUnity.Examples.Tests
{
    /// <summary>
    /// Wasmer will let you easily run Wasm module in a Rust host.
    /// 
    /// This example illustrates the basics of using Wasmer through a "Hello World"-like project:
    ///
    ///   1. How to load a Wasm modules as bytes
    ///   2. How to compile the module
    ///   3. How to create an instance of the module
    /// Original: https://github.com/wasmerio/wasmer/blob/master/examples/instance.rs
    /// </summary>
    [TestFixture]
    internal sealed class InstanceTest
    {
        [Test, RequiresPlayMode(false)]
        public void InstantiateModule()
        {
            // Let's declare the Wasm module.
            //
            // We are using the text representation of the module here but you can also load `.wasm`
            // files using the `include_bytes!` macro.
            var wasmBytes = @"
(module
    (type $add_one_t (func (param i32) (result i32)))
    (func $add_one_f (type $add_one_t) (param $value i32) (result i32)
        local.get $value
        i32.const 1
        i32.add)
    (export ""add_one"" (func $add_one_f))
)
            ".WatToWasm();

            // Create a Store.
            // Note that we don't need to specify the engine/compiler if we want to use
            // the default provided by Wasmer.
            // You can use `Store::default()` for that.
            using var store = Store.Default();

            // Let's compile the Wasm module.
            using var module = Module.New(store, wasmBytes);

            // Create an empty import object.
            using var importObject = ImportObject.New();

            // Let's instantiate the Wasm module.
            using var instance = Instance.New(store, module, importObject);

            // We now have an instance ready to be used.
            //
            // From an `Instance` we can retrieve any exported entities.
            // Each of these entities is covered in others examples.
            //
            // Here we are retrieving the exported function. We won't go into details here
            // as the main focus of this example is to show how to create an instance out
            // of a Wasm module and have basic interactions with it.
            var addOne = instance.GetFunction(store, "add_one");

            var result = addOne.Call<int, int>(1);

            result.Should().Be(2);
        }
    }
}