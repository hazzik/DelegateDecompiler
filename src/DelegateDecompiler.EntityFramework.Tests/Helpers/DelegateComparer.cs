using System;
using System.Collections;
using System.Collections.Generic;

namespace DelegateDecompiler.EntityFramework.Tests.Helpers
{
    class DelegateComparer<T> : IComparer, IComparer<T>
    {
        readonly Func<T, T, int> comparer;

        public DelegateComparer(Func<T, T, int> comparer)
        {
            this.comparer = comparer;
        }

        public int Compare(object x, object y)
        {
            return Compare((T) x, (T) y);
        }

        public int Compare(T x, T y)
        {
            return comparer(x, y);
        }
    }
}
