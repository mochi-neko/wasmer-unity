using System;

namespace Mochineko.WasmerBridge.OwnAttributes
{
    /// <summary>
    /// Passes ownership from callee to caller.
    /// </summary>
    [AttributeUsage(AttributeTargets.ReturnValue)]
    internal sealed class OwnResultAttribute : OwnAttribute
    {
    }
}