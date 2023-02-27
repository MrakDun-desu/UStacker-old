using System;
using System.Collections.Generic;
using UnityEngine;

namespace UStacker.Gameplay.Communication
{
    public class Mediator : MonoBehaviour
    {
        private readonly Dictionary<Type, MessageCollection> _registeredActions = new();

        public void Register<TMessage>(Action<TMessage> action, uint priority = 0)
            where TMessage : IMessage
        {
            var key = typeof(TMessage);
            if (!_registeredActions.ContainsKey(key)) _registeredActions[key] = new MessageCollection();
            
            _registeredActions[key].Add(action, priority);
        }

        public void Register(object action, Type type, uint priority = 0)
        {
            if (!typeof(IMessage).IsAssignableFrom(type)) return;
            if (!_registeredActions.ContainsKey(type)) _registeredActions[type] = new MessageCollection();

            _registeredActions[type].Add(action as Delegate, priority);
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
                Debug.Log($"Message collection was modified for {message}");
                throw;
            }
        }

        public void OnDestroy()
        {
            _registeredActions.Clear();
        }
    }
}