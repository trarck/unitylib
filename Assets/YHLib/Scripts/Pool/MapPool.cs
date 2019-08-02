using System;
using System.Collections.Generic;
using UnityEngine;

namespace YH.Pool
{
    public static class DictionaryPool<K,V>
    {
        // Object pool to avoid allocations.
        private static readonly ObjectPool<Dictionary<K,V>> s_DictionaryPool = new ObjectPool<Dictionary<K, V>>(null, l => l.Clear());

        public static Dictionary<K, V> Get()
        {
            return s_DictionaryPool.Get();
        }

        public static void Release(Dictionary<K, V> toRelease)
        {
            s_DictionaryPool.Release(toRelease);
        }
    }
}
