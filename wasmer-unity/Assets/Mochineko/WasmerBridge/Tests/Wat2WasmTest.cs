using FluentAssertions;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace Mochineko.WasmerBridge.Tests
{
    [TestFixture]
    internal sealed class Wat2WasmTest
    {
        [Test, RequiresPlayMode(false)]
        public void ConvertWat2WasmTest()
        {
            using var wasm = MockModule.EmptyWat.ToWasm();
            wasm.size.Should().Be((nuint)MockModule.EmptyWasmBinary.Length);
        }
        
        // TODO: Add error pattern
    }
}