using System;
using System.Collections.Generic;

namespace DelegateDecompiler
{
    internal class TypeHierarchyComparer : IComparer<Type>
    {
        public int Compare(Type x, Type y)
        {
            if (x == y) return 0;
            if (y.IsAssignableFrom(x)) return 1;
            if (x.IsAssignableFrom(y)) return -1;
            return 1;// string.CompareOrdinal(x.AssemblyQualifiedName, y.AssemblyQualifiedName);
        }
    }
}