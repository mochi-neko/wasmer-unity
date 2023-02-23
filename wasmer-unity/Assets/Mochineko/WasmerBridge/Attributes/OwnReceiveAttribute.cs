using System;

namespace Mochineko.WasmerBridge.Attributes
{
    /// <summary>
    /// Passes ownership from callee to caller.
    /// </summary>
    [AttributeUsage(AttributeTargets.ReturnValue | AttributeTargets.Property)]
    internal sealed class OwnReceiveAttribute : OwnAttribute
    {
    }
}