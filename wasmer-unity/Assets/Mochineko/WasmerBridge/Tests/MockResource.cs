namespace Mochineko.WasmerBridge.Tests
{
    internal static class MockResource
    {
        public const string EmptyWat = "(module)";

        public static byte[] EmptyWasmBinary => new byte[8]
        {
            0x00, 0x61, 0x73, 0x6d, // WASM_BINARY_MAGIC
            0x01, 0x00, 0x00, 0x00, // WASM_BINARY_VERSION
        };
        
        public const string HelloWorldWat = @"
(module
  (type $t0 (func))
  (import """" ""hello"" (func $.hello (type $t0)))
  (func $run
    call $.hello
  )
  (export ""run"" (func $run))
)";
    }
}