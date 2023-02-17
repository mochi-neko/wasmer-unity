using System;

namespace Mochineko.WasmerBridge.OwnAttributes
{
    /// <summary>
    /// Ownership of data by the Wasm API.
    /// Original explanation in c-api is in remarks.
    /// </summary>
    /// <remarks>
    /// The qualifier `own` is used to indicate ownership of data in this API.
    /// It is intended to be interpreted similar to a `const` qualifier:
    /// - `own wasm_xxx_t*` owns the pointed-to data
    /// - `own wasm_xxx_t` distributes to all fields of a struct or union `xxx`
    /// - `own wasm_xxx_vec_t` owns the vector as well as its elements(!)
    /// - an `own` function parameter passes ownership from caller to callee
    /// - an `own` function result passes ownership from callee to caller
    /// - an exception are `own` pointer parameters named `out`, which are copy-back
    ///   output parameters passing back ownership from callee to caller
    ///
    /// Own data is created by `wasm_xxx_new` functions and some others.
    /// It must be released with the corresponding `wasm_xxx_delete` function.
    ///
    /// Deleting a reference does not necessarily delete the underlying object,
    /// it merely indicates that this owner no longer uses it.
    ///
    /// For vectors, `const wasm_xxx_vec_t` is used informally to indicate that
    /// neither the vector nor its elements should be modified.
    /// TODO: introduce proper `wasm_xxx_const_vec_t`?
    /// </remarks>
    [AttributeUsage(AttributeTargets.All)]
    internal class OwnAttribute : Attribute
    {
    }
}