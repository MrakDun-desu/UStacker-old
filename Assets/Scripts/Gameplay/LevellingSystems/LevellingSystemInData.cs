using System;
using System.Collections.Generic;

namespace Blockstacker.Gameplay.LevellingSystems
{
    public class LevellingSystemInData
    {
        private readonly Dictionary<string, object> _data = new();

        public bool TryGetValue<T>(string name, out T value)
        {
            if (!_data.TryGetValue(name, out var data)) {
                value = default;
                return false;
            }
            if (data == null) {
                value = default;
                return false;
            }
            if (data is T) {
                value = default;
                return false;
            }
            value = (T)data;
            return true;
        }

        public void SetValue(string name, object value, bool notify = true)
        {
            _data[name] = value;
            if (notify) Changed?.Invoke(name);
        }

        public event Action<string> Changed;
    }
}