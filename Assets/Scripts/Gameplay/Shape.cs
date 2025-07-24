using Configuration;
using Core;
using Tools;
using UnityEngine;
using Zenject;

namespace Gameplay
{
    /// <summary>
    /// Логический компонент фигуры
    /// Отвечает за состояние, перемещение и основные игровые события, связанные с фигурой
    /// </summary>
    [RequireComponent(typeof(PoolableObject))]
    public class Shape : MonoBehaviour
    {
        public ShapeData ShapeData { get; private set; }
        public bool IsPlayerControlled { get; set; }

        private float _speed;
        private EventBus _eventBus;
        private PoolableObject _poolableObject;

        [Inject]
        public void Construct(EventBus eventBus)
        {
            _eventBus = eventBus;
        }

        private void Awake()
        {
            _poolableObject = GetComponent<PoolableObject>();
        }

        // Сброс состояния при получении из пула
        private void OnEnable()
        {
            IsPlayerControlled = false;
            _speed = 0f;
        }

        private void Update()
        {
            if (!IsPlayerControlled)
            {
                transform.Translate(Vector2.right * (_speed * Time.deltaTime));
            }
        }

        public void Initialize(ShapeData shapeData, float speed)
        {
            ShapeData = shapeData;
            _speed = speed;
        }

        // Вызывается из ShapeView, когда фигуру успешно поместили в слот
        public void ProcessCorrectDrop()
        {
            _speed = 0;
            _eventBus.Publish(new GameEvents.ShapeSortedCorrectly { ScoreToAdd = 1 });
        }

        // Вызывается из ShapeView, когда фигуру бросили в неверный слот
        public void ProcessIncorrectDrop()
        {
            _speed = 0;
            _eventBus.Publish(new GameEvents.ShapeSortedIncorrectly());
        }

        // Вызывается из DeathZone, когда фигура достигла конца пути
        public void ReachedDeathZone()
        {
            _speed = 0;
            _eventBus.Publish(new GameEvents.ShapeReachedDeathZone());
            GetComponent<ShapeView>().AnimateExplosion();
        }

        public void ReturnToPool()
        {
            if (_poolableObject.ParentPool != null)
            {
                _poolableObject.ParentPool.Return(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}