using System;

namespace Mochineko.WasmerBridge.OwnAttributes
{
    /// <summary>
    /// Passes ownership from caller to callee
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    internal sealed class OwnParameterAttribute : OwnAttribute
    {
    }
}