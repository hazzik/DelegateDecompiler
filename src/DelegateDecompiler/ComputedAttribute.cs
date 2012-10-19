using System;

namespace DelegateDecompiler
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public sealed class ComputedAttribute : Attribute
    {
    }
}