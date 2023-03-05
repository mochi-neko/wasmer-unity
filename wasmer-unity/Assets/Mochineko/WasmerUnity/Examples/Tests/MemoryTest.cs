using System.Runtime.InteropServices;
using FluentAssertions;
using Mochineko.WasmerUnity.Wasm;
using Mochineko.WasmerUnity.Wasm.Instances;
using Mochineko.WasmerUnity.Wasmer;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace Mochineko.WasmerUnity.Examples.Tests
{
    /// <summary>
    /// With Wasmer you'll be able to interact with guest module memory.
    ///
    /// This example illustrates the basics of interacting with Wasm module memory.:
    ///
    ///   1. How to load a Wasm modules as bytes
    ///   2. How to compile the module
    ///   3. How to create an instance of the module
    /// this example is a work in progress:
    /// TODO: clean it up and comment it https://github.com/wasmerio/wasmer/issues/1749
    /// </summary>
    [TestFixture]
    internal sealed class MemoryTest
    {
        [Test, RequiresPlayMode(false)]
        public void Memory()
        {
            // Let's declare the Wasm module.
            //
            // We are using the text representation of the module here but you can also load `.wasm`
            // files using the `include_bytes!` macro.
            var wasmBytes = @"
(module
    (type $mem_size_t (func (result i32)))
    (type $get_at_t (func (param i32) (result i32)))
    (type $set_at_t (func (param i32) (param i32)))
    (memory $mem 1)
    (func $get_at (type $get_at_t) (param $idx i32) (result i32)
        (i32.load (local.get $idx)))
    (func $set_at (type $set_at_t) (param $idx i32) (param $val i32)
        (i32.store (local.get $idx) (local.get $val)))
    (func $mem_size (type $mem_size_t) (result i32)
        (memory.size))
    (export ""get_at"" (func $get_at))
    (export ""set_at"" (func $set_at))
    (export ""mem_size"" (func $mem_size))
    (export ""memory"" (memory $mem))
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

            // The module exports some utility functions, let's get them.
            //
            // These function will be used later in this example.
            var memSize = instance.GetFunction(store, "mem_size");
            var getAt = instance.GetFunction(store, "get_at");
            var setAt = instance.GetFunction(store, "set_at");
            var memory = instance.GetMemory("memory");

            // We now have an instance ready to be used.
            //
            // We will start by querying the most interesting information
            // about the memory: its size. There are mainly two ways of getting
            // this:
            // * the size as a number of `Page`s
            // * the size as a number of bytes
            //
            // The size in bytes can be found either by querying its pages or by
            // querying the memory directly.
            memory.Size.Should().Be(1U);
            memory.DataSize.Should().Be(MemoryInstance.MemoryPageSize);

            // Sometimes, the guest module may also export a function to let you
            // query the memory. Here we have a `mem_size` function, let's try it:
            var result = memSize.Call<int>();

            ((uint)result).Should().Be(memory.Size);

            // Now that we know the size of our memory, it's time to see how wa
            // can change this.
            //
            // A memory can be grown to allow storing more things into it. Let's
            // see how we can do that:

            // Here we are requesting two more pages for our memory.
            memory.Grow(2);

            memory.Size.Should().Be(3);
            memory.DataSize.Should().Be(MemoryInstance.MemoryPageSize * 3);

            // Now that we know how to query and adjust the size of the memory,
            // let's see how wa can write to it or read from it.
            //
            // We'll only focus on how to do this using exported functions, the goal
            // is to show how to work with memory addresses. Here we'll use absolute
            // addresses to write and read a value.
            var memAddr = 0x2220;
            var val = 0xFEFEFFE;
            setAt.Call(memAddr, val);

            result = getAt.Call<int, int>(memAddr);
            result.Should().Be(val);

            // Now instead of using hard coded memory addresses, let's try to write
            // something at the end of the second memory page and read it.
            var pageSize = 0x1_0000;
            memAddr = pageSize * 2 - Marshal.SizeOf(val);
            val = 0xFEA09;
            setAt.Call(memAddr, val);

            result = getAt.Call<int, int>(memAddr);
            result.Should().Be(val);
        }
    }
}