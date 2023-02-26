using System;

namespace Mochineko.WasmerUnity.Wasm.Attributes
{
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
    internal sealed class ConstAttribute : Attribute
    {
    }
}