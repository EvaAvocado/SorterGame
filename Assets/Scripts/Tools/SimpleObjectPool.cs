using UnityEngine;
using System.Collections.Generic;
using Zenject;

namespace Tools
{
    /// <summary>
    /// Простая реализация паттерна "Пул объектов"
    /// Позволяет переиспользовать объекты вместо их постоянного создания/уничтожения
    /// </summary>
    public class SimpleObjectPool
    {
        private readonly GameObject _prefab;
        private readonly DiContainer _container;
        private readonly Transform _parent;
        private readonly Queue<GameObject> _pool = new Queue<GameObject>();

        public SimpleObjectPool(GameObject prefab, DiContainer container, Transform parent, int initialSize = 5)
        {
            _prefab = prefab;
            _container = container;
            _parent = parent;

            for (int i = 0; i < initialSize; i++)
            {
                CreateAndPoolObject();
            }
        }

        public GameObject Get()
        {
            if (_pool.Count == 0)
            {
                // Пул может расти, если потребуется больше объектов
                CreateAndPoolObject();
            }

            GameObject obj = _pool.Dequeue();
            obj.SetActive(true);
            return obj;
        }

        public void Return(GameObject obj)
        {
            obj.SetActive(false);
            _pool.Enqueue(obj);
        }

        private GameObject CreateAndPoolObject()
        {
            // Используем DiContainer для создания и инъекции зависимостей в префаб
            GameObject newObj = _container.InstantiatePrefab(_prefab, _parent);

            if (!newObj.TryGetComponent<PoolableObject>(out var poolable))
            {
                poolable = newObj.AddComponent<PoolableObject>();
            }

            poolable.ParentPool = this;

            newObj.SetActive(false);
            _pool.Enqueue(newObj);
            return newObj;
        }
    }
}