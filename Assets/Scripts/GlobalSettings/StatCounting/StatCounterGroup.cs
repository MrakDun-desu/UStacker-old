using System;
using System.Collections.Generic;
using System.Linq;
using Blockstacker.Common;
using Newtonsoft.Json;
using UnityEngine;

namespace Blockstacker.GlobalSettings.StatCounting
{
    [Serializable]
    public class StatCounterGroup
    {
        [JsonIgnore] [SerializeField] private string _name;
        [JsonIgnore] [SerializeField] private StringReferenceSO _gameType;
        [JsonIgnore] [SerializeField] private List<StatCounterSO> StatCounterSos = new();
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

        public List<StatCounterRecord> StatCounters
        {
            get
            {
                foreach (var counterSo in StatCounterSos.Where(
                             counterSo => !_statCounters.Exists(counter => counter.Equals(counterSo.Value))))
                {
                    _statCounters.Add(counterSo.Value);
                }

                return _statCounters;
            }
        }


        public StatCounterGroup Copy()
        {
            var output = new StatCounterGroup
            {
                _name = _name,
                _gameType = _gameType
            };

            foreach (var counter in StatCounters)
            {
                output.StatCounters.Add(counter.Copy());
            }

            return output;
        }
    }
}