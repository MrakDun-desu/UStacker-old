
/************************************
Mediator.cs -- created by Marek Dančo (xdanco00)
*************************************/
using System;
using System.Collections.Generic;
using UnityEngine;

namespace UStacker.Gameplay.Communication
{
    public class Mediator : MonoBehaviour
    {
        private readonly Dictionary<Type, MessageCollection> _registeredActions = new();

        public void OnDestroy()
        {
            _registeredActions.Clear();
        }

        public void Register<TMessage>(Action<TMessage> action, uint priority = 0)
            where TMessage : IMessage
        {
            var key = typeof(TMessage);
            _registeredActions.TryAdd(key, new MessageCollection());

            _registeredActions[key].Add(action, priority);
        }

        public void Register(Delegate action, Type type, uint priority = 0)
        {
            if (!typeof(IMessage).IsAssignableFrom(type)) return;
            _registeredActions.TryAdd(type, new MessageCollection());

            _registeredActions[type].Add(action, priority);
        }

        public void Unregister<TMessage>(Action<TMessage> action)
            where TMessage : IMessage
        {
            var key = typeof(TMessage);
            if (!_registeredActions.ContainsKey(key)) return;

            _registeredActions[key].Remove(action);
        }

        public void Send<TMessage>(TMessage message)
            where TMessage : IMessage
        {
            if (!_registeredActions.TryGetValue(message.GetType(), out var actions)) return;

            try
            {
                foreach (var action in actions)
                    action?.DynamicInvoke(message);
            }
            catch (InvalidOperationException)
            {
                Debug.Log($"Exception occured while sending {message}");
                throw;
            }
        }

        public void Clear()
        {
            _registeredActions.Clear();
        }
    }
}
/************************************
end Mediator.cs
*************************************/
