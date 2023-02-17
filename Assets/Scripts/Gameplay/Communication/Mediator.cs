using System;
using System.Collections.Generic;
using UnityEngine;

namespace UStacker.Gameplay.Communication
{
    public class Mediator : MonoBehaviour
    {
        private Dictionary<Type, List<Delegate>> _actions = new();

        private Dictionary<Type, List<Delegate>> RegisteredActions
        {
            get
            {
                _actions ??= _actions = new Dictionary<Type, List<Delegate>>();
                return _actions;
            }
        }

        public void Register<TMessage>(Action<TMessage> action, bool putFirst = false)
            where TMessage : IMessage
        {
            var key = typeof(TMessage);
            if (!RegisteredActions.ContainsKey(key)) RegisteredActions[key] = new List<Delegate>();
            if (putFirst)
                RegisteredActions[key].Insert(0, action);
            else
                RegisteredActions[key].Add(action);
        }

        public void Register(object action, Type type)
        {
            if (!typeof(IMessage).IsAssignableFrom(type)) return;
            if (!RegisteredActions.ContainsKey(type)) RegisteredActions[type] = new List<Delegate>();

            RegisteredActions[type].Add(action as Delegate);
        }
        
        public void Unregister<TMessage>(Action<TMessage> action)
            where TMessage : IMessage
        {
            var key = typeof(TMessage);
            if (!RegisteredActions.ContainsKey(key)) return;

            RegisteredActions[key].Remove(action);
        }

        public void Send<TMessage>(TMessage message)
            where TMessage : IMessage
        {
            if (!RegisteredActions.TryGetValue(message.GetType(), out var actions)) return;

            foreach (var action in actions) action?.DynamicInvoke(message);
        }

        public void OnDestroy()
        {
            RegisteredActions.Clear();
        }
    }
}