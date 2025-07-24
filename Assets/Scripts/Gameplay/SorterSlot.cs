using UnityEngine;
using Configuration;

namespace Gameplay
{
    /// <summary>
    /// Компонент-маркер для слота сортировки
    /// Просто хранит данные о том, какой тип фигуры принимает
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class SorterSlot : MonoBehaviour
    {
        [SerializeField] private ShapeData _slotShapeData;
        public ShapeData SlotShapeData => _slotShapeData;
    }
}