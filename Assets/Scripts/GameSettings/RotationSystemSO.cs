
/************************************
RotationSystemSO.cs -- created by Marek Dančo (xdanco00)
*************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Serialization;
using UStacker.Common;

namespace UStacker.GameSettings
{
    [CreateAssetMenu(fileName = "Rotation", menuName = "UStacker/Rotation system")]
    public class RotationSystemSO : ScriptableObject, ISerializationCallbackReceiver
    {
        [SerializeField] private List<KeyValuePair> _kickTables = new();
        [SerializeField] private List<KeyValuePair> _duplicateTables = new();

        [FormerlySerializedAs("DefaultKickTable")] [SerializeField]
        private KickTable KickTable = new();

        public readonly RotationSystem RotationSystem = new();

        public void OnBeforeSerialize()
        {
            KickTable = RotationSystem.DefaultTable;
            _kickTables.Clear();
            foreach (var keyValuePair in RotationSystem.KickTables)
                _kickTables.Add(new KeyValuePair(keyValuePair.Key, keyValuePair.Value));
        }

        public void OnAfterDeserialize()
        {
            RotationSystem.DefaultTable = KickTable;
            RotationSystem.KickTables.Clear();

            foreach (var keyValuePair in _kickTables)
                if (RotationSystem.KickTables.ContainsKey(keyValuePair.Key))
                {
                    _duplicateTables.Add(keyValuePair);
                    _kickTables.Remove(keyValuePair);
                }
                else
                {
                    RotationSystem.KickTables.Add(keyValuePair.Key, keyValuePair.Value);
                }

            foreach (var keyValuePair in _duplicateTables.Where(keyValuePair =>
                         !RotationSystem.KickTables.ContainsKey(keyValuePair.Key)))
            {
                _duplicateTables.Remove(keyValuePair);
                RotationSystem.KickTables.Add(keyValuePair.Key, keyValuePair.Value);
            }
        }

        [ContextMenu("Copy JSON to clipboard")]
        public void CopyToClipboard()
        {
            GUIUtility.systemCopyBuffer =
                JsonConvert.SerializeObject(RotationSystem, StaticSettings.DefaultSerializerSettings);
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
/************************************
end RotationSystemSO.cs
*************************************/
