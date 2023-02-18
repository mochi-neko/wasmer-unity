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
            MockResource.EmptyWat.FromWatToWasm(out var wasm);
            using (wasm)
            {
                wasm.size.Should().Be((nuint)binary.Length);

                wasm.ToManagedSpan(out var excluded);
                for (var i = 0; i < binary.Length; i++)
                {
                    excluded[i].Should().Be(binary[i]);
                }
            }
        }
    }
}