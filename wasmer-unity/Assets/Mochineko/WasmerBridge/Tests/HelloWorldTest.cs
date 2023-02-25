using System;
using FluentAssertions;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace Mochineko.WasmerBridge.Tests
{
    [TestFixture]
    internal unsafe sealed class HelloWorldTest
    {
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
            using var engine = Engine.New();

            // Create store.
            using var store = Store.New(engine);

            // Compile wasm.
            using var module = Module.FromWat(store, wat);

            // Define "hello" function as import object.
            // TODO: Improve interface to create ExternalInstanceVector.
            using var functionType = FunctionType.New(
                Array.Empty<ValueKind>(),
                Array.Empty<ValueKind>());
            bool helloCalled = false;
            using var functionInstance = FunctionInstance.New(
                store,
                functionType,
                callback: (_, _) =>
                {
                    helloCalled = true;
                    return IntPtr.Zero;
                });
            using var externalInstance = ExternalInstance.FromFunctionWithOwnership(functionInstance);
            var externalInstances = new[] { externalInstance };

            ExternalInstanceVector.New(externalInstances, out var imports);
            using (imports)
            {
                // Instantiate module.
                using var instance = Instance.New(store, module, in imports);

                // Get exported function.
                //var run = instance.ExportFunction<Unit>(store, "run");

                // Call exported function.
                //run.Invoke();

                // Assert flag.
                helloCalled.Should().Be(true);
            }
        }
        
    }
}