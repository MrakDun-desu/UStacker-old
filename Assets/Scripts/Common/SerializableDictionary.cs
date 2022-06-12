using System;
using System.Collections.Generic;
using UnityEngine;

namespace Blockstacker.Common
{
    [Serializable]
    public abstract class SerializableDictionary<TKey, TValue> : ISerializationCallbackReceiver
    {
        public readonly Dictionary<TKey, TValue> Content = new();

        [Tooltip("Actual content of the dictionary")]
        [SerializeField] private List<Entry> _entries = new();
        [Tooltip("Aren't actually in the dictionary, but are required to make serialization possible")]
        [SerializeField] private List<Entry> _duplicateEntries = new();

        public void OnBeforeSerialize()
        {
            _entries.Clear();
            foreach (var keyValuePair in Content)
            {
                _entries.Add(new Entry(keyValuePair.Key, keyValuePair.Value));
            }
        }

        public void OnAfterDeserialize()
        {
            Content.Clear();

            for (var i = 0; i < _entries.Count; i++)
            {
                var keyValuePair = _entries[i];
                if (Content.ContainsKey(keyValuePair.Key))
                {
                    _duplicateEntries.Add(keyValuePair);
                    _entries.Remove(keyValuePair);
                }
                else
                    Content.Add(keyValuePair.Key, keyValuePair.Value);
            }

            for (var i = 0; i < _duplicateEntries.Count; i++)
            {
                var keyValuePair = _duplicateEntries[i];
                if (Content.ContainsKey(keyValuePair.Key)) continue;
                
                _duplicateEntries.Remove(keyValuePair);
                Content.Add(keyValuePair.Key, keyValuePair.Value);
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