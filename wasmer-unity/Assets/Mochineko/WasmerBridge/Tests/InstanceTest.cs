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
        public unsafe void InstantiateModuleWithFunctionTest()
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
                
                // TODO: Improve interface to create ExternalInstanceVector.
                using var functionType = FunctionType.New(
                    Array.Empty<ValueKind>(),
                    Array.Empty<ValueKind>());
                bool importFunctionCalled = false;
                using var importedFunction = FunctionInstance.New(
                    store,
                    functionType,
                    callback: (_, _) =>
                    {
                        importFunctionCalled = true;
                        return IntPtr.Zero;
                    });
                using var externalInstance = ExternalInstance.FromFunction(importedFunction);
                var externalInstances = new[] { externalInstance };

                ExternalInstanceVector.New(externalInstances, out var imports);
                using (imports)
                {
                    using var instance = Instance.New(store, module, in imports);
                    instance.Should().NotBeNull();
                    instance.Exports(out var exports);
                    using (exports)
                    {
                        exports.size.Should().Be((nuint)externalInstances.Length);
                        exports.ToManaged(out var exportInstances);
                        using var export = exportInstances[0];
                        export.Should().NotBeNull();

                        using var exportedFunction = export.ToFunction();
                        exportedFunction.Should().NotBeNull();
                        
                        // TODO: Improve interface to run function.
                        ValueInstanceVector.NewEmpty(out var arguments);
                        ValueInstanceVector.NewEmpty(out var results);
                        using (arguments)
                        using (results)
                        {
                            using var trap = importedFunction.Call(in arguments, ref results);
                            trap.Should().BeNull();
                            
                            importFunctionCalled.Should().BeTrue();
                            
                            // TODO: Improve ownership management
                            importedFunction.Handle.SetHandleAsInvalid();
                        }
                    }
                }
            }

            GC.Collect();
        }
    }
}