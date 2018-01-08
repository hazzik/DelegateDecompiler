using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DelegateDecompiler
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public sealed class ComputedAttribute : NotMappedAttribute
    {
    }
}