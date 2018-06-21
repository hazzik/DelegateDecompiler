using System;
using System.Collections.Generic;
using System.Linq;

namespace DelegateDecompiler
{
    static class TypeHierarchy
    {
        public static IEnumerable<Type> Traverse(Type root, IEnumerable<Type> ancestors)
        {
            var result = new List<Type>();
            var children = ancestors.ToLookup(t => t.BaseType);
            if (!root.IsInterface)
            {
                Traverse(result, root, children);                
            }
            else
            {
                foreach (var hierarchy in children.Where(l => !root.IsAssignableFrom(l.Key)))
                {
                    Traverse(result, hierarchy.Key, children);
                }
            }

            return result;
        }

        private static void Traverse(ICollection<Type> result, Type type, ILookup<Type, Type> children)
        {
            foreach (var child in children[type].OrderBy(t => t.Name))
            {
                result.Add(child);

                Traverse(result, child, children);
            }
        }
    }
}
