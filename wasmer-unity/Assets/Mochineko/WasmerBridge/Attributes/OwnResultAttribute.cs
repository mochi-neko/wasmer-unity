using System;

namespace Mochineko.WasmerBridge.Attributes
{
    /// <summary>
    /// Passes ownership from callee to caller.
    /// </summary>
    [AttributeUsage(AttributeTargets.ReturnValue)]
    internal sealed class OwnResultAttribute : OwnAttribute
    {
    }
}