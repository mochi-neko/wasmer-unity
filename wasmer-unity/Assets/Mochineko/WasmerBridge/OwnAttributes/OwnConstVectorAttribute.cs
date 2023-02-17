using System;

namespace Mochineko.WasmerBridge.OwnAttributes
{
    /// <summary>
    /// For vectors, `const wasm_xxx_vec_t` is used informally to indicate that
    /// neither the vector nor its elements should be modified.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
    internal sealed class OwnConstVectorAttribute : OwnAttribute
    {
    }
}