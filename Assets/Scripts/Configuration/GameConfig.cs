using System.Collections.Generic;
using UnityEngine;

namespace Configuration
{
    /// <summary>
    /// ScriptableObject, который хранит все основные настройки и баланс игры
    /// Позволяет геймдизайнерам легко изменять параметры без вмешательства в код
    /// </summary>
    [CreateAssetMenu(fileName = "GameConfig", menuName = "Game/New Game Config")]
    public class GameConfig : ScriptableObject
    {
        [Header("Game Balance")]
        [field: SerializeField] public int PlayerHealth { get; private set; } = 3;
        [field: SerializeField] public Vector2Int ShapesToWinRange { get; private set; } = new Vector2Int(15, 25);
        [field: SerializeField] public Vector2 SpawnDelayRange { get; private set; } = new Vector2(0.8f, 2.0f);
        [field: SerializeField] public Vector2 ShapeSpeedRange { get; private set; } = new Vector2(2.0f, 4.0f);

        [Header("Shapes")]
        [Tooltip("Список всех возможных фигур в игре. Спаунер будет выбирать из них случайным образом.")]
        [field: SerializeField]
        public List<ShapeData> AllShapes { get; private set; }

        [Header("Animation & FX")]
        [field: SerializeField]
        public AnimationSettings Animations { get; private set; }
    }

    /// <summary>
    /// Вспомогательный класс для хранения настроек анимации
    /// </summary>
    [System.Serializable]
    public class AnimationSettings
    {
        [Header("Shape Animations")] public float ShapeAppearDuration = 0.3f;
        public float ShapeGrabScale = 1.2f;
        public float ShapeGrabDuration = 0.15f;
        public float ReturnToLaneDuration = 0.3f;
        public float AnimateIntoSlotMoveDuration = 0.2f;
        public float AnimateIntoSlotScaleDuration = 0.25f;

        [Header("UI Animations")] public float UIPulseDuration = 0.2f;
        public float ScorePulseScale = 1.25f;
        public float HealthPulseScale = 0.8f;
    }
}