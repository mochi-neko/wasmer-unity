using System;

namespace Mochineko.WasmerBridge.Attributes
{
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
    internal sealed class ConstAttribute : Attribute
    {
    }
}