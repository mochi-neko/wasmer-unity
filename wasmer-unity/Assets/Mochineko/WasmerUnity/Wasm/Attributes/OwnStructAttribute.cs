using System;

namespace Mochineko.WasmerUnity.Wasm.Attributes
{
    /// <summary>
    /// <para>
    /// Distributes to all fields of a struct or union.
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
    [AttributeUsage(AttributeTargets.Struct)]
    internal sealed class OwnStructAttribute : OwnAttribute
    {
    }
}