using System;

namespace Mochineko.WasmerUnity.Wasm.Attributes
{
    /// <summary>
    /// Passes ownership from callee to caller.
    /// </summary>
    [AttributeUsage(AttributeTargets.ReturnValue | AttributeTargets.Property)]
    internal sealed class OwnReceiveAttribute : OwnAttribute
    {
    }
}