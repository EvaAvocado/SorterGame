using UnityEngine;
using Zenject;
using Core;
using DG.Tweening;

namespace Tools
{
    /// <summary>
    /// Отвечает за эффект тряски камеры
    /// </summary>
    public class CameraShaker : MonoBehaviour
    {
        [Header("Shake Parameters")] [SerializeField]
        private float _shakeDuration = 0.3f;

        [SerializeField] private float _shakeStrength = 0.2f;
        [SerializeField] private int _vibrato = 10;
        [SerializeField] private float _randomness = 90f;

        private EventBus _eventBus;
        private Sequence _shakeSequence;

        [Inject]
        public void Construct(EventBus eventBus)
        {
            _eventBus = eventBus;
        }

        private void OnEnable()
        {
            _eventBus.Subscribe<GameEvents.ShapeSortedIncorrectly>(HandleShakeEvent);
            _eventBus.Subscribe<GameEvents.ShapeReachedDeathZone>(HandleShakeEvent);
        }

        private void OnDisable()
        {
            _eventBus.Unsubscribe<GameEvents.ShapeSortedIncorrectly>(HandleShakeEvent);
            _eventBus.Unsubscribe<GameEvents.ShapeReachedDeathZone>(HandleShakeEvent);
            _shakeSequence?.Kill();
        }

        private void HandleShakeEvent<T>(T e) where T : struct
        {
            Shake();
        }

        public void Shake()
        {
            _shakeSequence?.Kill();
            _shakeSequence = DOTween.Sequence()
                .Append(transform.DOShakePosition(_shakeDuration, _shakeStrength, _vibrato, _randomness))
                .SetUpdate(true);
        }
    }
}