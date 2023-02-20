using System;
using System.Collections.Generic;
using UnityEngine;

namespace UStacker.Gameplay.Communication
{
    public class Mediator : MonoBehaviour
    {
        private readonly Dictionary<Type, List<Delegate>> _registeredActions = new();

        public void Register<TMessage>(Action<TMessage> action, bool putFirst = false)
            where TMessage : IMessage
        {
            var key = typeof(TMessage);
            if (!_registeredActions.ContainsKey(key)) _registeredActions[key] = new List<Delegate>();
            if (putFirst)
                _registeredActions[key].Insert(0, action);
            else
                _registeredActions[key].Add(action);
        }

        public void Register(object action, Type type)
        {
            if (!typeof(IMessage).IsAssignableFrom(type)) return;
            if (!_registeredActions.ContainsKey(type)) _registeredActions[type] = new List<Delegate>();

            _registeredActions[type].Add(action as Delegate);
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

            foreach (var action in actions) action?.DynamicInvoke(message);
        }

        public void OnDestroy()
        {
            _registeredActions.Clear();
        }
    }
}