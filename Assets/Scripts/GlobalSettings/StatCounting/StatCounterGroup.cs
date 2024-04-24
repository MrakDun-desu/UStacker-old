
/************************************
StatCounterGroup.cs -- created by Marek Dančo (xdanco00)
*************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

namespace UStacker.GlobalSettings.StatCounting
{
    [Serializable]
    public class StatCounterGroup : ISerializationCallbackReceiver
    {
        [JsonIgnore] [SerializeField] private string _name;
        [JsonIgnore] [SerializeField] private List<StatCounterSO> _statCounterSos = new();
        [JsonIgnore] [SerializeField] private List<StatCounterRecord> _statCounters = new();

        public string Name
        {
            get => _name;
            set => _name = value;
        }

        public List<StatCounterRecord> StatCounters => _statCounters;

        public void OnBeforeSerialize()
        {
            foreach (var counterSo in _statCounterSos.Where(counterSo => counterSo is not null))
            {
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
                _name = _name
            };

            foreach (var counter in StatCounters) output.StatCounters.Add(counter.Copy());

            return output;
        }
    }
}
/************************************
end StatCounterGroup.cs
*************************************/
