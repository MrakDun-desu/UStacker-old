using System;
using System.Collections.Generic;

namespace Blockstacker.Gameplay.Communication
{
    public static class Mediator
    {
        private static readonly Dictionary<Type, List<Delegate>> _registeredActions = new();

        public static void Register<TMessage>(Action<TMessage> action)
            where TMessage : IMessage
        {
            var key = typeof(TMessage);
            if (!_registeredActions.ContainsKey(key)) {
                _registeredActions[key] = new();
            }
            _registeredActions[key].Add(action);
        }

        public static void Unregister<TMessage>(Action<TMessage> action)
            where TMessage : IMessage
        {
            var key = typeof(TMessage);
            if (!_registeredActions.ContainsKey(key)) return;

            _registeredActions[key].Remove(action);
        }

        public static void Send<TMessage>(TMessage message)
            where TMessage : IMessage
        {
            if (!_registeredActions.TryGetValue(message.GetType(), out var actions)) return;

            foreach (var action in actions) {
                action?.DynamicInvoke(message);
            }
        }
        
        public static void Clear() {
            _registeredActions.Clear();
        }
    }
}