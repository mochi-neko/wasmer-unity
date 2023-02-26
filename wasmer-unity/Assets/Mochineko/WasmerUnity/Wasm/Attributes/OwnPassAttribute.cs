using System;

namespace Mochineko.WasmerUnity.Wasm.Attributes
{
    /// <summary>
    /// Passes ownership from caller to callee.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    internal sealed class OwnPassAttribute : OwnAttribute
    {
    }
}