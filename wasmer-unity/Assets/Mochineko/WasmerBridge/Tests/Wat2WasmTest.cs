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
            var binary = MockResource.EmptyWasmBinary;
            using var wasmFromWat = MockResource.EmptyWat.FromWatToWasm();
            wasmFromWat.size.Should().Be((nuint)binary.Length);

            ByteVector.ToManagedSpan(wasmFromWat, out var excluded);
            for (var i = 0; i < binary.Length; i++)
            {
                excluded[i].Should().Be(binary[i]);
            }
        }
    }
}