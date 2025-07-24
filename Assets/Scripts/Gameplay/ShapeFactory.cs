using Configuration;
using UnityEngine;
using Zenject;
using Tools;
using System.Collections.Generic;

namespace Gameplay
{
    public interface IShapeFactory
    {
        Shape Create(ShapeData shapeData, Vector3 position);
    }

    /// <summary>
    /// Фабрика, отвечающая за создание и первоначальную настройку экземпляров фигур
    /// Управляет пулами объектов для каждого типа фигур, чтобы избежать постоянного создания/уничтожения
    /// </summary>
    public class ShapeFactory : IShapeFactory
    {
        private readonly GameConfig _config;
        private readonly DiContainer _container;
        private readonly Dictionary<ShapeData, SimpleObjectPool> _pools;
        private readonly Transform _poolsParent;

        private const string PoolsContainerName = "[Pools_Container]";

        public ShapeFactory(GameConfig config, DiContainer container)
        {
            _config = config;
            _container = container;
            _pools = new Dictionary<ShapeData, SimpleObjectPool>();
            _poolsParent = new GameObject(PoolsContainerName).transform;
        }

        public Shape Create(ShapeData shapeData, Vector3 position)
        {
            // Ленивое создание пула при первом запросе фигуры такого типа
            if (!_pools.ContainsKey(shapeData))
            {
                if (shapeData.Prefab == null)
                {
                    Debug.LogError($"У ShapeData '{shapeData.name}' не назначен префаб!");
                    return null;
                }

                var newPool = new SimpleObjectPool(shapeData.Prefab, _container, _poolsParent);
                _pools.Add(shapeData, newPool);
            }

            GameObject shapeObject = _pools[shapeData].Get();
            shapeObject.transform.position = position;
            shapeObject.transform.rotation = Quaternion.identity;

            Shape newShape = shapeObject.GetComponent<Shape>();
            ShapeView shapeView = shapeObject.GetComponent<ShapeView>();

            if (newShape != null && shapeView != null)
            {
                // Инициализируем логическую и визуальную части
                float randomSpeed = Random.Range(_config.ShapeSpeedRange.x, _config.ShapeSpeedRange.y);
                newShape.Initialize(shapeData, randomSpeed);

                Sprite randomSprite = shapeData.GetRandomSprite();
                Color randomColor = ColorTools.GetRandomHSVColor();
                shapeView.InitializeVisuals(randomSprite, randomColor);
            }

            return newShape;
        }
    }
}