namespace Mochineko.WasmerBridge.Tests
{
    internal static class MockModule
    {
        public const string EmptyWat = "(module)";

        public static readonly byte[] EmptyWasmBinary = new byte[8]
        {
            0x00, 0x61, 0x73, 0x6d, // WASM_BINARY_MAGIC
            0x01, 0x00, 0x00, 0x00, // WASM_BINARY_VERSION
        };
    }
}