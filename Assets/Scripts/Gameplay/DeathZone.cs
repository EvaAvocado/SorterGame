using UnityEngine;

namespace Gameplay
{
    /// <summary>
    /// Компонент, представляющий "Смертельную зону" в конце лайна
    /// При столкновении с фигурой инициирует событие потери жизни
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class DeathZone : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent(out Shape shape))
            {
                // Игнорируем фигуру, если ее тащит игрок
                if (shape.IsPlayerControlled)
                {
                    return;
                }

                shape.ReachedDeathZone();
            }
        }
    }
}