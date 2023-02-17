using System;

namespace Mochineko.WasmerBridge.Attributes
{
    [AttributeUsage(AttributeTargets.Parameter)]
    internal sealed class ConstAttribute : Attribute
    {
    }
}