using UnityEngine;

namespace _00._Work.Resources._04._Templates.PoolManager
{
    public interface IPoolable
    {
        public string ItemName { get; }
        public GameObject GameObject { get; }
        public void ResetItem();
    }
}