using System;
using System.Collections.Generic;
using UnityEngine;

namespace UStacker.Gameplay.InputProcessing
{
    [Serializable]
    public class UpdateEvent : IComparable<UpdateEvent>
    {
        // static field so there is absolutely no chance events have the same priority
        private static int LastPriority;
        private readonly List<UpdateEvent> _parentList;
        private readonly int _priority;
        [SerializeField] private EventType _type;
        public EventType Type => _type;
        
        [SerializeField] private double _time = double.PositiveInfinity;
        public double Time
        {
            get => _time;
            set
            {
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (_time == value)
                    return;
                
                _time = value;
                _parentList.Sort();
            }
        }

        public UpdateEvent(List<UpdateEvent> parentList, EventType type)
        {
            _parentList = parentList;
            _parentList.Add(this);
            _priority = LastPriority++;
            _type = type;
        }


        public int CompareTo(UpdateEvent other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;
            var output = _time.CompareTo(other._time);
            return output == 0 ? _priority.CompareTo(other._priority) : output;
        }
    }
}