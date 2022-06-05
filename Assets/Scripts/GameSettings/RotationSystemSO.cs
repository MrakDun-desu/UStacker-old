using System;
using System.Collections.Generic;
using System.Linq;
using Blockstacker.Common.Enums;
using UnityEngine;

namespace Blockstacker.GameSettings
{
    [CreateAssetMenu(fileName = "Rotation", menuName = "Blockstacker/Rotation system")]
    public class RotationSystemSO : ScriptableObject, ISerializationCallbackReceiver
    {
        [SerializeField] private List<KeyValuePair> _entries = new();
        [SerializeField] private List<KeyValuePair> _duplicateEntries = new();
        
        public readonly RotationSystem RotationSystem = new();

        public void OnBeforeSerialize()
        {
            _entries.Clear();
            foreach (var keyValuePair in RotationSystem.KickTables)
            {
                _entries.Add(new KeyValuePair(keyValuePair.Key, keyValuePair.Value));
            }
        }

        public void OnAfterDeserialize()
        {
            RotationSystem.KickTables.Clear();
            foreach (var keyValuePair in _entries)
            {
                if (RotationSystem.KickTables.ContainsKey(keyValuePair.Key))
                {
                    _duplicateEntries.Add(keyValuePair);
                    _entries.Remove(keyValuePair);
                }
                else
                    RotationSystem.KickTables.Add(keyValuePair.Key, keyValuePair.Value);
            }

            foreach (var keyValuePair in _duplicateEntries.Where(keyValuePair => !RotationSystem.KickTables.ContainsKey(keyValuePair.Key)))
            {
                _duplicateEntries.Remove(keyValuePair);
                RotationSystem.KickTables.Add(keyValuePair.Key, keyValuePair.Value);
            }
        }
        
        [Serializable]
        public struct KeyValuePair
        {
            public PieceType Key;
            public KickTable Value;

            public KeyValuePair(PieceType key, KickTable value)
            {
                Key = key;
                Value = value;
            }
        }

    }
}