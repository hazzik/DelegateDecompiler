﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace DelegateDecompiler
{
    static class TypeHierarchy
    {
        public static IEnumerable<Type> Traverse(Type root, IEnumerable<Type> descendants)
        {
            var children = descendants.ToLookup(t => t.BaseType.SafeGetGenericTypeDefinition());

            var result = new List<Type>();
            var usher = new HashSet<Type>();
            if (!root.IsInterface)
            {
                Traverse(result, usher, root, children);
            }
            else
            {
                foreach (var hierarchy in children.Where(l => !root.IsAssignableFrom(l.Key)))
                {
                    Traverse(result, usher, hierarchy.Key, children);
                }
            }

            return result;
        }

        private static void Traverse(ICollection<Type> result, HashSet<Type> usher, Type type,
            ILookup<Type, Type> children)
        {
            var types = BuildChildren(type, children)
                .Where(t => !t.ContainsGenericParameters)
                .OrderBy(t => t.Name);

            foreach (var child in types)
            {
                if (usher.Add(child))
                {
                    result.Add(child);

                    Traverse(result, usher, child, children);
                }
            }
        }

        static IEnumerable<Type> BuildChildren(Type type, ILookup<Type, Type> children)
        {
            if (!type.IsGenericType)
            {
                return children[type];
            }

            var genericArguments = type.GetGenericArguments();

            return children[type.GetGenericTypeDefinition()]
                .Select(c => TryMakeGenericType(c, genericArguments));
        }

        static Type TryMakeGenericType(Type c, Type[] genericArguments)
        {
            if (c.IsGenericTypeDefinition && c.GetGenericArguments().Length == genericArguments.Length)
            {
                return c.MakeGenericTypeFromClosedParentArguments(genericArguments);
            }

            return c;
        }
    }
}
