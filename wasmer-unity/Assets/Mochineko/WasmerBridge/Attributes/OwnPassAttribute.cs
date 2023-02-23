using System;

namespace Mochineko.WasmerBridge.Attributes
{
    /// <summary>
    /// Passes ownership from caller to callee.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    internal sealed class OwnPassAttribute : OwnAttribute
    {
    }
}