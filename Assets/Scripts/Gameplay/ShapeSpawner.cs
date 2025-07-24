using Configuration;
using UnityEngine;

namespace Gameplay
{
    public interface IShapeSpawner
    {
        void SpawnShape(Transform[] spawnPoints);
    }

    /// <summary>
    /// Класс с логикой спавна: решает, какую фигуру и с какими параметрами создавать
    /// </summary>
    public class ShapeSpawner : IShapeSpawner
    {
        private readonly IShapeFactory _shapeFactory;
        private readonly GameConfig _config;

        public ShapeSpawner(IShapeFactory factory, GameConfig config)
        {
            _shapeFactory = factory;
            _config = config;
        }

        public void SpawnShape(Transform[] spawnPoints)
        {
            if (_config.AllShapes == null || _config.AllShapes.Count == 0)
            {
                Debug.LogError("В GameConfig не указаны фигуры для спавна!");
                return;
            }

            ShapeData randomShapeData = _config.AllShapes[Random.Range(0, _config.AllShapes.Count)];
            Transform randomSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

            _shapeFactory.Create(randomShapeData, randomSpawnPoint.position);
        }
    }
}