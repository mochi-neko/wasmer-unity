namespace Mochineko.WasmerBridge
{
    internal enum ValueKind : byte
    {
        Int32,
        Int64,
        Float32,
        Float64,
        AnyRef = 128,
        FuncRef,
    }
}