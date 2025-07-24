using System.Collections.Generic;
using UnityEngine;

namespace Configuration
{
    /// <summary>
    /// ScriptableObject, определяющий конкретный тип фигуры
    /// Хранит ссылку на префаб и возможные варианты спрайтов, позволяет гибко настраивать типы фигур в редакторе
    /// </summary>
    [CreateAssetMenu(fileName = "NewShapeData", menuName = "Game/Shape Data")]
    public class ShapeData : ScriptableObject
    {
        [Header("Shape Configuration")] 
        [SerializeField] private GameObject _prefab;
        [SerializeField] private List<Sprite> _sprites;

        public GameObject Prefab => _prefab;

        public Sprite GetRandomSprite()
        {
            if (_sprites == null || _sprites.Count == 0)
            {
                // Возвращаем null, чтобы фигура использовала спрайт из префаба по умолчанию
                return null;
            }

            return _sprites[Random.Range(0, _sprites.Count)];
        }
    }
}