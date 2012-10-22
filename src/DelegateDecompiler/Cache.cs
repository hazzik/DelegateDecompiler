using System;
using System.Collections.Generic;

namespace DelegateDecompiler
{
    class Cache<TKey, TValue>
    {
        readonly IDictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>();
        readonly object @lock = new object();

        public TValue GetOrAdd(TKey key, Func<TKey, TValue> func)
        {
            TValue value;
            if (dictionary.TryGetValue(key, out value))
                return value;

            lock (@lock)
            {
                if (dictionary.TryGetValue(key, out value))
                    return value;

                value = func(key);
                dictionary.Add(key, value);
            }

            return value;
        }

    }
}