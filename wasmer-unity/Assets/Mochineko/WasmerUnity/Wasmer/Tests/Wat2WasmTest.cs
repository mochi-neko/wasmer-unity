using System;
using FluentAssertions;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace Mochineko.WasmerUnity.Wasmer.Tests
{
    [TestFixture]
    internal sealed class Wat2WasmTest
    {
        [Test, RequiresPlayMode(false)]
        public void ConvertWat2WasmTest()
        {
            var binary = MockResource.EmptyWasmBinary;
            var wasm = MockResource.EmptyWat.ToWasm();

            wasm.Length.Should().Be(binary.Length);

            for (var i = 0; i < binary.Length; i++)
            {
                wasm[i].Should().Be(binary[i]);
            }

            GC.Collect();
        }
    }
}