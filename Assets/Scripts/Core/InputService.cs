using UnityEngine;
using Zenject;
using Gameplay;

namespace Core
{
    public interface IInputService
    {
        bool IsDragging { get; }
        Vector3 GetWorldPosition();
        void Tick();
    }

    /// <summary>
    /// Сервис, отвечающий за обработку пользовательского ввода (клики, перетаскивания)
    /// Абстрагирует конкретные методы ввода от остальной части игры
    /// </summary>
    public class InputService : IInputService
    {
        public bool IsDragging => _draggedShapeView != null;

        private Camera _mainCamera;
        private ShapeView _draggedShapeView;

        [Inject]
        public void Construct(Camera mainCamera)
        {
            _mainCamera = mainCamera;
        }

        public void Tick()
        {
            if (Input.GetMouseButtonDown(0))
            {
                HandlePress();
            }
            else if (Input.GetMouseButtonUp(0))
            {
                HandleRelease();
            }

            if (IsDragging)
            {
                _draggedShapeView.HandleDrag(GetWorldPosition());
            }
        }

        public Vector3 GetWorldPosition()
        {
            return _mainCamera.ScreenToWorldPoint(Input.mousePosition);
        }

        private void HandlePress()
        {
            Vector3 worldPos = GetWorldPosition();
            RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);

            if (hit.collider != null && hit.collider.TryGetComponent(out ShapeView shapeView))
            {
                _draggedShapeView = shapeView;
                _draggedShapeView.StartDrag();
            }
        }

        private void HandleRelease()
        {
            if (_draggedShapeView != null)
            {
                _draggedShapeView.EndDrag();
                _draggedShapeView = null;
            }
        }
    }
}