using UnityEngine;
using UnityEngine.Pool;

namespace DevelopKit.BasicTemplate
{
    public enum PoolMode
    {
        Stack,
        LinkedList,
    }
    
    public sealed class GameObjectPool<T> where T : MonoBehaviour
    {
        private const int DEFAULT_MAXCAPACITY = int.MaxValue - 1;
        
        private IObjectPool<T> _pool;
        
        private string prefabKey;
        private Transform parent;
        private int maxCapacity;
        private int currentCapacity;
        
        public void Bind(string prefabKey, Transform parent = null, PoolMode mode = PoolMode.Stack, int maxCapacity = DEFAULT_MAXCAPACITY)
        {
            this.prefabKey = prefabKey;
            this.parent = parent;
            this.maxCapacity = maxCapacity;
            currentCapacity = 0;
            
            switch (mode)
            {
                case PoolMode.Stack:
                    _pool = new ObjectPool<T>(CreatePooledItem, GetPooledItem, ReleasePooledItem, DestroyPooledItem);
                    break;
                case PoolMode.LinkedList:
                    _pool = new LinkedPool<T>(CreatePooledItem, GetPooledItem, ReleasePooledItem, DestroyPooledItem);
                    break;
            }
        }

        private T CreatePooledItem()
        {
            return AddressableUtil.Instantiate<T>(prefabKey, parent);
        }
        
        private void GetPooledItem(T item)
        {
            item.gameObject.SetActive(true);
        }
        
        private void ReleasePooledItem(T item)
        {
            item.gameObject.SetActive(false);
        }
        
        private void DestroyPooledItem(T item)
        {
            AddressableUtil.Release(item);
        }

        public T Get()
        {
            if (currentCapacity <= 0)
            {
                return CreatePooledItem();
            }
            
            currentCapacity--;
            return _pool.Get();
        }
        
        public void Release(T item)
        {
            if (currentCapacity >= maxCapacity)
            {
                DestroyPooledItem(item);
                return;
            }
            
            currentCapacity++;
            _pool.Release(item);
        }
    }
}