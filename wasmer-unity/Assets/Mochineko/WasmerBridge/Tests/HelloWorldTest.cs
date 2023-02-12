using System;
using FluentAssertions;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace Mochineko.WasmerBridge.Tests
{
    internal sealed class HelloWorldTest
    {
        private const string EmptyWat = "(module)";

        private static readonly byte[] EmptyWasmBinary = new byte[8]
        {
            0x00, 0x61, 0x73, 0x6d, // WASM_BINARY_MAGIC
            0x01, 0x00, 0x00, 0x00, // WASM_BINARY_VERSION
        };

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
        public unsafe void InitializeNativeArrayTest()
        {
            using var emptyArray = WasmByteArray.New();
            emptyArray.Should().NotBeNull();
            emptyArray.size.Should().Be((nuint)0);

            fixed (byte* ptr = EmptyWasmBinary)
            {
                using var array = WasmByteArray.New((nuint)EmptyWasmBinary.Length, ptr);
                array.Should().NotBeNull();
                array.size.Should().Be((nuint)EmptyWasmBinary.Length);
            }

            using var converted = WasmByteArray.ToNativeArray(EmptyWasmBinary);
            converted.Should().NotBeNull();
            converted.size.Should().Be((nuint)EmptyWasmBinary.Length);
        }

        [Test, RequiresPlayMode(false)]
        public void ConvertWat2WasmTest()
        {
            using var wasm = EmptyWat.ToWasm();
            wasm.size.Should().Be((nuint)EmptyWasmBinary.Length);
        }
        
        [Test, RequiresPlayMode(false)]
        public void ValidateWasmBinaryTest()
        {
            using var engine = new Engine();
            using var store = new Store(engine);
            using var wasm = WasmByteArray.ToNativeArray(EmptyWasmBinary);

            Module.Validate(store, wasm).Should().BeTrue();

            using var notWasm = WasmByteArray.ToNativeArray(new byte[8]
                {
                    0x00, 0x00, 0x00, 0x00,
                    0x00, 0x00, 0x00, 0x00,
                }
            );
            
            Module.Validate(store, notWasm).Should().BeFalse();
        }
        
        [Test, RequiresPlayMode(false)]
        public void InitializeModuleFromWatTest()
        {
            using var engine = new Engine();
            using var store = new Store(engine);
            using var wasm = EmptyWat.ToWasm();

            using var module = new Module(store, "empty", wasm);
            module.Should().NotBeNull();
        }

        [Test, RequiresPlayMode(false)]
        public void InitializeModuleTest()
        {
            using var engine = new Engine();
            using var store = new Store(engine);
            using var wasm = WasmByteArray.ToNativeArray(EmptyWasmBinary);

            using var module = new Module(store, "empty", wasm);
            module.Should().NotBeNull();
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
            var instance = module.Instantiate(store, module, importObject);

            // Get exported function.
            var run = instance.ExportFunction<Unit>(store, "run");

            // Call exported function.
            run.Invoke();

            // Assert flag.
            helloCalled.Should().Be(true);
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