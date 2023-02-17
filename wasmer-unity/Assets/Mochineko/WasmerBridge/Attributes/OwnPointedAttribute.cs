using System;

namespace Mochineko.WasmerBridge.Attributes
{
    /// <summary>
    /// <para>
    /// Owns the pointed-to data.
    /// </para>
    /// <para>
    /// Own data is created by `wasm_xxx_new` functions and some others.
    /// It must be released with the corresponding `wasm_xxx_delete` function.
    /// </para>
    /// <para>
    /// Deleting a reference does not necessarily delete the underlying object,
    /// it merely indicates that this owner no longer uses it.
    /// </para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    internal sealed class OwnPointedAttribute : OwnAttribute
    {
    }
}