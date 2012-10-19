using System;

namespace DelegateDecompiller
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
    public sealed class DecompileAttribute : Attribute
    {
    }
}
