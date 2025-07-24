using System.Threading.Tasks;
using System.Threading;
using Configuration;
using Core;
using UnityEngine;
using Zenject;

namespace Gameplay
{
    /// <summary>
    /// MonoBehaviour, который запускает и контролирует процесс спавна фигур с течением времени
    /// Использует асинхронный подход для создания задержек между появлениями фигур
    /// </summary>
    public class SpawnerView : MonoBehaviour
    {
        [SerializeField] private Transform[] _spawnPoints;

        private IShapeSpawner _spawner;
        private GameConfig _config;
        private EventBus _eventBus;
        private CancellationTokenSource _cts;

        [Inject]
        public void Construct(IShapeSpawner spawner, GameConfig config, EventBus eventBus)
        {
            _spawner = spawner;
            _config = config;
            _eventBus = eventBus;
        }

        private void OnDestroy()
        {
            _cts?.Cancel();
            _cts?.Dispose();
        }

        public void StartSpawning(int shapesToSpawn)
        {
            _cts?.Cancel();
            _cts = new CancellationTokenSource();
            SpawnLoop(shapesToSpawn, _cts.Token).WrapErrors();
        }

        private async Task SpawnLoop(int shapesToSpawn, CancellationToken token)
        {
            for (int i = 0; i < shapesToSpawn; i++)
            {
                if (token.IsCancellationRequested) return;

                float delay = Random.Range(_config.SpawnDelayRange.x, _config.SpawnDelayRange.y);
                await Task.Delay((int)(delay * 1000), token);

                if (token.IsCancellationRequested) return;

                _spawner.SpawnShape(_spawnPoints);
            }

            _eventBus.Publish(new GameEvents.AllShapesProcessed());
        }
    }

    /// <summary>
    /// Вспомогательный класс для безопасной обработки исключений
    /// в асинхронных методах, запущенных из не асинхронного контекста
    /// </summary>
    public static class TaskExtensions
    {
        public static async void WrapErrors(this Task task)
        {
            try
            {
                await task;
            }
            catch (System.Exception ex)
            {
                Debug.LogException(ex);
            }
        }
    }
}