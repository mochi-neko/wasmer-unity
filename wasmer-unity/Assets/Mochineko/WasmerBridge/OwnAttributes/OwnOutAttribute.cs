using System;

namespace Mochineko.WasmerBridge.OwnAttributes
{
    /// <summary>
    /// `own` pointer parameters named `out`, which are copy-back
    /// output parameters passing back ownership from callee to caller.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    internal sealed class OwnOutAttribute : OwnAttribute
    {
    }
}