namespace Mochineko.WasmerUnity.Wasm
{
    public enum ValueKind : byte
    {
        Int32 = 0,
        Int64 = 1,
        Float32 = 2,
        Float64 = 3,
        AnyRef = 128,
        FuncRef = 129,
    }
}