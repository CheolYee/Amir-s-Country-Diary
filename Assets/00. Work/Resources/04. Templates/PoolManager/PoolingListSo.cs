using System;
using System.Collections.Generic;
using UnityEngine;

namespace _00._Work.Resources._04._Templates.PoolManager
{
    [Serializable]

    [CreateAssetMenu(fileName = "PoolingList", menuName = "SO/Pool/List", order = 0)]
    public class PoolingListSo : ScriptableObject
    {
        public List<PoolItem> items;
    }
}