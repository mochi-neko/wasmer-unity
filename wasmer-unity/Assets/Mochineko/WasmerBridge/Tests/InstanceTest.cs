using System;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace Mochineko.WasmerBridge.Tests
{
    [TestFixture]
    internal sealed class InstanceTest
    {
        [Test, RequiresPlayMode(false)]
        [Ignore("Not implemented")]
        public void InstantiateTest()
        {
            using var engine = Engine.New();
            using var store = Store.New(engine);
            ByteVector.New(MockResource.EmptyWasmBinary, out var wasm);
            using (wasm)
            {
                using var module = Module.New(store, in wasm);
                //using var instance = Instance.New(store, module,);
            }
            
            GC.Collect();
        }
    }
}