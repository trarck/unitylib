using System;
using System.Collections.Generic;
using UnityEngine;

namespace YH.Pool
{
    public class PoolManager
    {
        static List<IObjectPool> pools = new List<IObjectPool>();

        public static void AddPool(IObjectPool pool)
        {
            pools.Add(pool);
        }

        public static int GetPoolsCount()
        {
            return pools.Count;
        }
    }
}
