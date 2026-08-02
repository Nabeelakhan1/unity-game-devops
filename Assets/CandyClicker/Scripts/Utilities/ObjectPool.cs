using System.Collections.Generic;
using UnityEngine;

namespace CandyClicker.Utilities
{
    /// <summary>
    /// Minimal prefab pool. Grows on demand, never destroys.
    /// Usage: new ObjectPool&lt;FloatingText&gt;(prefab, 16, transform);
    /// </summary>
    public class ObjectPool<T> where T : MonoBehaviour
    {
        private readonly Queue<T> _pool = new Queue<T>();
        private readonly T _prefab;
        private readonly Transform _parent;

        public ObjectPool(T prefab, int initialSize, Transform parent = null)
        {
            _prefab = prefab;
            _parent = parent;

            for (int i = 0; i < initialSize; i++)
                Release(Create());
        }

        /// <summary>Takes an active instance out of the pool.</summary>
        public T Get()
        {
            T instance = _pool.Count > 0 ? _pool.Dequeue() : Create();
            instance.gameObject.SetActive(true);
            return instance;
        }

        /// <summary>Deactivates the instance and puts it back.</summary>
        public void Release(T instance)
        {
            if (instance == null) return;

            instance.gameObject.SetActive(false);
            _pool.Enqueue(instance);
        }

        private T Create()
        {
            T instance = Object.Instantiate(_prefab, _parent);
            instance.gameObject.SetActive(false);
            return instance;
        }
    }
}
