using System;
using System.Reflection;

namespace DelegateDecompiler
{
    internal static class MemberExtensions
    {
        public static Type FieldOrPropertyType(this MemberInfo member)
        {
            switch (member)
            {
                case FieldInfo field:
                    return field.FieldType;
                case PropertyInfo property:
                    return property.PropertyType;
                default:
                    throw new NotSupportedException($"MemberInfo {member} is not supported");
            }
        }
    }
}
