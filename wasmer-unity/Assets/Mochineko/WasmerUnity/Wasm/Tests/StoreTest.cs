using System;
using Mochineko.WasmerUnity.Wasm;
using FluentAssertions;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace Mochineko.WasmerUnity.Wasm.Tests
{
    [TestFixture]
    internal sealed class StoreTest
    {
        [Test, RequiresPlayMode(false)]
        public void CreateStoreTest()
        {
            using var engine = Engine.New();

            using var store = Store.New(engine);
            store.Should().NotBeNull();
            
            GC.Collect();
        }   
    }
}