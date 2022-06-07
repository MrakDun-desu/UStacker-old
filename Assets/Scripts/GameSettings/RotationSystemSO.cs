using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Blockstacker.GameSettings
{
    [CreateAssetMenu(fileName = "Rotation", menuName = "Blockstacker/Rotation system")]
    public class RotationSystemSO : ScriptableObject, ISerializationCallbackReceiver
    {
        [SerializeField] private List<KeyValuePair> _kickTables = new();
        [SerializeField] private List<KeyValuePair> _duplicateTables = new();

        [SerializeField] private KickTable DefaultKickTable = new();

        public readonly RotationSystem RotationSystem = new();

        public void OnBeforeSerialize()
        {
            DefaultKickTable = RotationSystem.DefaultTable;
            _kickTables.Clear();
            foreach (var keyValuePair in RotationSystem.StringKickTables)
            {
                _kickTables.Add(new KeyValuePair(keyValuePair.Key, keyValuePair.Value));
            }
        }

        public void OnAfterDeserialize()
        {
            RotationSystem.DefaultTable = DefaultKickTable;
            RotationSystem.StringKickTables.Clear();
            
            foreach (var keyValuePair in _kickTables)
            {
                if (RotationSystem.StringKickTables.ContainsKey(keyValuePair.Key))
                {
                    _duplicateTables.Add(keyValuePair);
                    _kickTables.Remove(keyValuePair);
                }
                else
                    RotationSystem.StringKickTables.Add(keyValuePair.Key, keyValuePair.Value);
            }

            foreach (var keyValuePair in _duplicateTables.Where(keyValuePair => !RotationSystem.StringKickTables.ContainsKey(keyValuePair.Key)))
            {
                _duplicateTables.Remove(keyValuePair);
                RotationSystem.StringKickTables.Add(keyValuePair.Key, keyValuePair.Value);
            }
        }
        
        [Serializable]
        public struct KeyValuePair
        {
            public string Key;
            public KickTable Value;
            
            public KeyValuePair(string key, KickTable value)
            {
                Key = key;
                Value = value;
            }
            
        }

    }
}