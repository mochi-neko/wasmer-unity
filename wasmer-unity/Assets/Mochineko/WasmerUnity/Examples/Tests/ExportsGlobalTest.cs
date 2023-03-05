using System;
using FluentAssertions;
using Mochineko.WasmerUnity.Wasm;
using Mochineko.WasmerUnity.Wasm.Instances;
using Mochineko.WasmerUnity.Wasmer;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace Mochineko.WasmerUnity.Examples.Tests
{
    /// <summary>
    /// A Wasm module can export entities, like functions, memories,
    /// globals and tables.
    ///
    /// This example illustrates how to use exported globals. They come
    /// in 2 flavors:
    ///
    ///   1. Immutable globals (const),
    ///   2. Mutable globals.
    /// Original: https://github.com/wasmerio/wasmer/blob/master/examples/exports_global.rs
    /// </summary>
    [TestFixture]
    internal sealed class ExportsGlobalTest
    {
        [Test, RequiresPlayMode(false)]
        public void ExportsGlobal()
        {
            // Let's declare the Wasm module with the text representation.
            var wasmBytes = @"
(module
    (global $one (export ""one"") f32 (f32.const 1))
    (global $some (export ""some"") (mut f32) (f32.const 0))
    (func (export ""get_one"") (result f32) (global.get $one))
    (func (export ""get_some"") (result f32) (global.get $some))
    (func (export ""set_some"") (param f32) (global.set $some (local.get 0)))
)"
                .WatToWasm();

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

            // Here we go.
            //
            // The Wasm module exports some globals. Let's get them.
            // Note that
            //
            //     ```
            //     get_global(name)
            //     ```
            //
            // is just an alias to
            //
            //     ```
            //     get::<Global>(name)`.
            //     ```
            using var one = instance.GetGlobal("one");
            using var some = instance.GetGlobal("some");
            
            // Let's get the globals types. The results are `GlobalType`s.
            using var oneType = one.Type;
            using var someType = some.Type;

            oneType.Mutability.Should().Be(Mutability.Constant);
            oneType.Content.Kind.Should().Be(ValueKind.Float32);

            someType.Mutability.Should().Be(Mutability.Variable);
            someType.Content.Kind.Should().Be(ValueKind.Float32);
            
            // Getting the values of globals can be done in two ways:
            //   1. Through an exported function,
            //   2. Using the Global API directly.
            //
            // We will use an exported function for the `one` global
            // and the Global API for `some`.
            var getOne = instance.GetFunction(store, "get_one");

            var oneValue = getOne.Call<float>();
            var someValue = some.Get();

            oneValue.Should().Be(1.0f);
            someValue.OfFloat32.Should().Be(0f);
            
            // Trying to set the value of a immutable global (`const`)
            // will result in a `RuntimeError`.
            Action setValueToConstant = () =>
            {
                one.Set(ValueInstance.NewFloat32(42.0f));
            };
            setValueToConstant.Should().Throw<InvalidOperationException>();

            var oneResult = one.Get();

            oneResult.OfFloat32.Should().Be(1.0f);
            
            // Setting the values of globals can be done in two ways:
            //   1. Through an exported function,
            //   2. Using the Global API directly.
            //
            // We will use both for the `some` global.
            var setSome = instance.GetFunction(store, "set_some");
            
            setSome.Call(21.0f);
            
            var someResult = some.Get();

            someResult.OfFloat32.Should().Be(21.0f);
            
            some.Set(ValueInstance.NewFloat32(42.0f));
            
            someResult = some.Get();

            someResult.OfFloat32.Should().Be(42.0f);
        }
    }
}