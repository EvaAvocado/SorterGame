using System;
using System.Collections.Generic;

namespace Core
{
    /// <summary>
    /// Реализация паттерна "Шина событий" для слабой связи между компонентами системы
    /// Позволяет отправлять и получать события, не имея прямых ссылок друг на друга
    /// </summary>
    public class EventBus
    {
        private readonly Dictionary<Type, Delegate> _events = new Dictionary<Type, Delegate>();

        public void Subscribe<T>(Action<T> listener) where T : struct
        {
            Type eventType = typeof(T);
            if (_events.TryGetValue(eventType, out Delegate d))
            {
                _events[eventType] = Delegate.Combine(d, listener);
            }
            else
            {
                _events[eventType] = listener;
            }
        }

        public void Unsubscribe<T>(Action<T> listener) where T : struct
        {
            Type eventType = typeof(T);
            if (_events.TryGetValue(eventType, out Delegate d))
            {
                Delegate currentDelegate = Delegate.Remove(d, listener);
                if (currentDelegate == null)
                {
                    _events.Remove(eventType);
                }
                else
                {
                    _events[eventType] = currentDelegate;
                }
            }
        }

        public void Publish<T>(T eventData) where T : struct
        {
            Type eventType = typeof(T);
            if (_events.TryGetValue(eventType, out Delegate d))
            {
                (d as Action<T>)?.Invoke(eventData);
            }
        }
    }
}