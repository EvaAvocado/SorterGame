using Configuration;
using DG.Tweening;
using Tools;
using UnityEngine;
using Zenject;

namespace Gameplay
{
    /// <summary>
    /// Визуальный компонент фигуры
    /// Отвечает за рендеринг, обработку перетаскивания, анимации и визуальные эффекты
    /// </summary>
    [RequireComponent(typeof(Collider2D), typeof(SpriteRenderer), typeof(Shape))]
    public class ShapeView : MonoBehaviour
    {
        [Header("FX")] [SerializeField] private GameObject _explosionParticlePrefab;
        [SerializeField] [Range(0f, 1f)] private float _hoverDarkenFactor = 0.75f;

        private GameConfig _config;
        private Shape _shape;
        private SpriteRenderer _spriteRenderer;
        private Collider2D _collider;
        private Color _originalColor;
        private Vector3 _startDragPositionOnLane;
        private bool _isDragging = false;
        private static SimpleObjectPool _particlePool;
        private static Transform _vfxContainer;

        private const string VFXContainerName = "[VFX_Container]";

        [Inject]
        public void Construct(GameConfig config, DiContainer container)
        {
            _config = config;

            // Пул для эффектов взрыва
            if (_particlePool == null && _explosionParticlePrefab != null)
            {
                _vfxContainer = new GameObject(VFXContainerName).transform;
                _particlePool = new SimpleObjectPool(_explosionParticlePrefab, container, _vfxContainer, 5);
            }
        }

        private void Awake()
        {
            _shape = GetComponent<Shape>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _collider = GetComponent<Collider2D>();
        }

        // Сброс визуального состояния при получении из пула
        private void OnEnable()
        {
            _collider.enabled = true;
            _spriteRenderer.enabled = true;
            _isDragging = false;
            transform.localScale = Vector3.zero;
        }

        public void InitializeVisuals(Sprite sprite, Color color)
        {
            _spriteRenderer.sprite = sprite ? sprite : _spriteRenderer.sprite;
            _originalColor = color;
            _spriteRenderer.color = _originalColor;

            transform.DOScale(Vector3.one, _config.Animations.ShapeAppearDuration).SetEase(Ease.OutBack);
        }

        #region Drag & Drop

        public void StartDrag()
        {
            if (_isDragging) return;
            _isDragging = true;
            _shape.IsPlayerControlled = true;
            _startDragPositionOnLane = transform.position;

            transform.DOScale(_config.Animations.ShapeGrabScale, _config.Animations.ShapeGrabDuration)
                .SetEase(Ease.OutBack);
        }

        public void HandleDrag(Vector3 worldPosition)
        {
            if (!_isDragging) return;
            transform.position = new Vector3(worldPosition.x, worldPosition.y, 0);
        }

        public void EndDrag()
        {
            if (!_isDragging) return;
            _isDragging = false;

            transform.DOScale(1.0f, _config.Animations.ShapeGrabDuration);

            SorterSlot closestSlot = FindClosestSlot();

            if (closestSlot != null && closestSlot.SlotShapeData == _shape.ShapeData)
            {
                _shape.ProcessCorrectDrop();
                AnimateIntoSlot(closestSlot.transform.position);
            }
            else if (closestSlot != null) // Неверный слот
            {
                _shape.ProcessIncorrectDrop();
                AnimateExplosion();
            }
            else // Слот не найден
            {
                ReturnToLane();
            }
        }

        private SorterSlot FindClosestSlot()
        {
            Collider2D[] colliders = Physics2D.OverlapPointAll(transform.position);
            SorterSlot closestSlot = null;
            float minDistance = float.MaxValue;

            foreach (var col in colliders)
            {
                if (col.TryGetComponent(out SorterSlot slot))
                {
                    float distance = Vector2.Distance(transform.position, slot.transform.position);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        closestSlot = slot;
                    }
                }
            }

            return closestSlot;
        }

        #endregion

        #region Animations & Visual Feedback

        // Приятные анимации, дающие обратную связь
        private void ReturnToLane()
        {
            _shape.IsPlayerControlled = true;
            _spriteRenderer.color = _originalColor;

            transform.DOMove(_startDragPositionOnLane, _config.Animations.ReturnToLaneDuration)
                .SetEase(Ease.OutCubic)
                .OnComplete(() => _shape.IsPlayerControlled = false);
        }

        public void AnimateIntoSlot(Vector3 slotPosition)
        {
            _collider.enabled = false;
            _spriteRenderer.color = _originalColor;

            var sequence = DOTween.Sequence();
            sequence.Append(transform.DOMove(slotPosition, _config.Animations.AnimateIntoSlotMoveDuration)
                .SetEase(Ease.OutCubic));
            sequence.Append(transform.DOScale(Vector3.zero, _config.Animations.AnimateIntoSlotScaleDuration)
                .SetEase(Ease.InBack));
            sequence.OnComplete(_shape.ReturnToPool);
        }

        public void AnimateExplosion()
        {
            _collider.enabled = false;
            _spriteRenderer.enabled = false;

            if (_particlePool != null)
            {
                GameObject particleInstance = _particlePool.Get();
                particleInstance.transform.position = transform.position;
                var ps = particleInstance.GetComponent<ParticleSystem>();
                if (ps != null)
                {
                    var main = ps.main;
                    main.startColor = _originalColor;
                    ps.Play();
                }
            }

            _shape.ReturnToPool();
        }

        // Затемнение при наведении на слот
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_isDragging && other.TryGetComponent(out SorterSlot _))
            {
                _spriteRenderer.color = ColorTools.GetDarkerColor(_originalColor, _hoverDarkenFactor);
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (_isDragging && other.TryGetComponent(out SorterSlot _))
            {
                _spriteRenderer.color = _originalColor;
            }
        }

        #endregion
    }
}