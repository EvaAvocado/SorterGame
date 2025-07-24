using Configuration;
using Gameplay;
using UnityEngine;
using Zenject;

namespace Core
{
    /// <summary>
    /// Главная точка входа в игровую логику
    /// Отвечает за управление игровым циклом, состоянием (очки, жизни) и условиями победы/поражения
    /// </summary>
    public class GameEntryPoint : IInitializable, System.IDisposable, ITickable
    {
        private readonly IInputService _inputService;
        private readonly EventBus _eventBus;
        private readonly GameConfig _config;
        private readonly SpawnerView _spawnerView;

        private int _currentScore;
        private int _currentHealth;
        private int _shapesToWin;
        private int _shapesProcessed;
        private bool _gameEnded;

        public GameEntryPoint(EventBus eventBus, GameConfig config, SpawnerView spawnerView, IInputService inputService)
        {
            _eventBus = eventBus;
            _config = config;
            _spawnerView = spawnerView;
            _inputService = inputService;
        }

        public void Initialize()
        {
            SubscribeToEvents();
            StartNewGame();
        }

        public void Dispose()
        {
            UnsubscribeFromEvents();
        }
        
        public void Tick()
        {
            _inputService.Tick();
        }

        private void StartNewGame()
        {
            _gameEnded = false;
            _currentScore = 0;
            _shapesProcessed = 0;
            _currentHealth = _config.PlayerHealth;
            _shapesToWin = Random.Range(_config.ShapesToWinRange.x, _config.ShapesToWinRange.y + 1);

            _eventBus.Publish(new GameEvents.ScoreUpdated { NewScore = _currentScore });
            _eventBus.Publish(new GameEvents.HealthUpdated { NewHealth = _currentHealth });

            _spawnerView.StartSpawning(_shapesToWin);
        }

        private void SubscribeToEvents()
        {
            _eventBus.Subscribe<GameEvents.ShapeSortedCorrectly>(OnShapeSortedCorrectly);
            _eventBus.Subscribe<GameEvents.ShapeSortedIncorrectly>(OnShapeProcessFailed);
            _eventBus.Subscribe<GameEvents.ShapeReachedDeathZone>(OnShapeProcessFailed);
            _eventBus.Subscribe<GameEvents.AllShapesProcessed>(OnAllShapesSpawned);
        }

        private void UnsubscribeFromEvents()
        {
            _eventBus.Unsubscribe<GameEvents.ShapeSortedCorrectly>(OnShapeSortedCorrectly);
            _eventBus.Unsubscribe<GameEvents.ShapeSortedIncorrectly>(OnShapeProcessFailed);
            _eventBus.Unsubscribe<GameEvents.ShapeReachedDeathZone>(OnShapeProcessFailed);
            _eventBus.Unsubscribe<GameEvents.AllShapesProcessed>(OnAllShapesSpawned);
        }


        private void OnAllShapesSpawned(GameEvents.AllShapesProcessed e)
        {
        }

        private void OnShapeSortedCorrectly(GameEvents.ShapeSortedCorrectly e)
        {
            if (_gameEnded) return;
            _currentScore += e.ScoreToAdd;
            _shapesProcessed++;
            _eventBus.Publish(new GameEvents.ScoreUpdated { NewScore = _currentScore });
            CheckEndCondition();
        }

        private void OnShapeProcessFailed<T>(T e) where T : struct
        {
            HandleFailedProcess();
        }

        private void HandleFailedProcess()
        {
            if (_gameEnded) return;
            _currentHealth--;
            _shapesProcessed++;
            _eventBus.Publish(new GameEvents.HealthUpdated { NewHealth = _currentHealth });

            if (_currentHealth <= 0)
            {
                _gameEnded = true;
                _eventBus.Publish(new GameEvents.GameLost());
            }
            else
            {
                CheckEndCondition();
            }
        }

        private void CheckEndCondition()
        {
            if (_gameEnded || _shapesProcessed < _shapesToWin) return;

            _gameEnded = true;
            if (_currentHealth > 0)
            {
                _eventBus.Publish(new GameEvents.GameWon { FinalScore = _currentScore });
            }
            else
            {
                _eventBus.Publish(new GameEvents.GameLost());
            }
        }
    }
}