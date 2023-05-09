
/************************************
SerializableDictionary.cs -- created by Marek Dančo (xdanco00)
*************************************/
using System;
using System.Collections.Generic;
using UnityEngine;

namespace UStacker.Common
{
    [Serializable]
    public abstract class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>,
        ISerializationCallbackReceiver
    {
        [Tooltip("Actual content of the dictionary")] [SerializeField]
        private List<Entry> _entries = new();

        [Tooltip("Aren't actually in the dictionary, but are required to make serialization possible")] [SerializeField]
        private List<Entry> _duplicateEntries = new();

        public void OnBeforeSerialize()
        {
            _entries.Clear();
            foreach (var keyValuePair in this) _entries.Add(new Entry(keyValuePair.Key, keyValuePair.Value));
        }

        public void OnAfterDeserialize()
        {
            Clear();

            for (var i = 0; i < _entries.Count; i++)
            {
                var keyValuePair = _entries[i];
                if (ContainsKey(keyValuePair.Key))
                {
                    _duplicateEntries.Add(keyValuePair);
                    _entries.Remove(keyValuePair);
                }
                else
                {
                    Add(keyValuePair.Key, keyValuePair.Value);
                }
            }

            for (var i = 0; i < _duplicateEntries.Count; i++)
            {
                var keyValuePair = _duplicateEntries[i];
                if (ContainsKey(keyValuePair.Key)) continue;

                _duplicateEntries.Remove(keyValuePair);
                Add(keyValuePair.Key, keyValuePair.Value);
            }
        }

        [Serializable]
        public class Entry
        {
            public TKey Key;
            public TValue Value;

            public Entry(TKey key, TValue value)
            {
                Key = key;
                Value = value;
            }
        }
    }
}
/************************************
end SerializableDictionary.cs
*************************************/
