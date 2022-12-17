using System;
using System.Collections.Generic;
using Blockstacker.Common;
using Newtonsoft.Json;
using UnityEngine;

namespace Blockstacker.GlobalSettings.StatCounting
{
    [Serializable]
    public class StatCounterGroup : ISerializationCallbackReceiver
    {
        [JsonIgnore] [SerializeField] private string _name;
        [JsonIgnore] [SerializeField] private StringReferenceSO _gameType;
        [JsonIgnore] [SerializeField] private List<StatCounterSO> _statCounterSos = new();
        [JsonIgnore] [SerializeField] private List<StatCounterRecord> _statCounters = new();

        public string Name
        {
            get => _gameType == null ? _name : _gameType.Value;
            set
            {
                _gameType = null;
                _name = value;
            }
        }

        public List<StatCounterRecord> StatCounters => _statCounters;

        public void OnBeforeSerialize()
        {
            foreach (var counterSo in _statCounterSos)
            {
                if (counterSo is null) continue;

                bool Predicate(StatCounterRecord counter)
                {
                    return counter.Name == counterSo.Value.Name;
                }

                if (!_statCounters.Exists(Predicate))
                    _statCounters.Add(counterSo.Value.Copy());
                else
                    _statCounters.Find(Predicate).Script = counterSo.Value.Script;
            }
            for (var i = 0; i < _statCounters.Count; i++)
            {
                var counter = _statCounters[i];
                if (_statCounterSos.Exists(so => so.Value.Name == counter.Name)) continue;
                _statCounters.RemoveAt(i);
                i--;
            }
        }

        public void OnAfterDeserialize()
        {
        }


        public StatCounterGroup Copy()
        {
            var output = new StatCounterGroup
            {
                _name = _name, _gameType = _gameType
            };

            foreach (var counter in StatCounters) output.StatCounters.Add(counter.Copy());

            return output;
        }
    }
}