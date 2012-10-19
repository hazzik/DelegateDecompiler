using System;

namespace DelegateDecompiller
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
    public sealed class DecompileAttribute : Attribute
    {
    }
}
