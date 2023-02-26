namespace Mochineko.WasmerUnity.Wasm
{
    internal enum ExternalKind : byte
    {
        Function,
        Global,
        Table,
        Memory,
    }
}