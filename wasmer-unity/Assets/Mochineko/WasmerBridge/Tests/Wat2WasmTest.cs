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
            using var wasmFromWat = MockResource.EmptyWat.ToWasm();
            wasmFromWat.size.Should().Be((nuint)MockResource.EmptyWasmBinary.Length);

            ByteVector.ToManagedSpan(wasmFromWat, out var binary);
            for (var i = 0; i < MockResource.EmptyWasmBinary.Length; i++)
            {
                binary[i].Should().Be(MockResource.EmptyWasmBinary[i]);
            }
        }
    }
}