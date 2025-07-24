using Configuration;
using Core;
using Gameplay;
using Tools;
using UI;
using UnityEngine;

namespace Zenject
{
    /// <summary>
    /// Главный инсталлятор зависимостей Zenject для игровой сцены
    /// Связывает все интерфейсы, сервисы и компоненты в единый DI-контейнер
    /// </summary>
    public class GameInstaller : MonoInstaller
    {
        [SerializeField] private GameConfig _gameConfig;
        [SerializeField] private SpawnerView _spawnerView;
        [SerializeField] private UIController _uiController;
        [SerializeField] private CameraShaker _cameraShaker;

        public override void InstallBindings()
        {
            // Конфигурация
            Container.Bind<GameConfig>().FromInstance(_gameConfig).AsSingle();
            Container.Bind<Camera>().FromInstance(Camera.main).AsSingle();

            // Основные системы
            Container.Bind<EventBus>().AsSingle();
            Container.BindInterfacesAndSelfTo<GameEntryPoint>().AsSingle().NonLazy();

            // Система ввода
            Container.Bind<IInputService>().To<InputService>().AsSingle();

            // Инструменты
            Container.Bind<CameraShaker>().FromInstance(_cameraShaker).AsSingle();

            // Фабрика и Спавнер
            Container.Bind<IShapeFactory>().To<ShapeFactory>().AsSingle();
            Container.Bind<IShapeSpawner>().To<ShapeSpawner>().AsSingle();

            // View компоненты со сцены
            Container.Bind<SpawnerView>().FromInstance(_spawnerView).AsSingle();
            Container.Bind<UIController>().FromInstance(_uiController).AsSingle();
        }
    }
}