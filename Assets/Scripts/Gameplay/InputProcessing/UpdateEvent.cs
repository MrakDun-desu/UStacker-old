﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace UStacker.Gameplay.InputProcessing
{
    [Serializable]
    public class UpdateEvent : IComparable<UpdateEvent>
    {
        private static int LastPriority = 0;
        private readonly List<UpdateEvent> _parentList;
        private readonly int _priority;
        [SerializeField] private EventType _type;
        public EventType Type => _type;
        
        [SerializeField] private double _time;
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

        public UpdateEvent(List<UpdateEvent> parentList, double time, EventType type)
        {
            _parentList = parentList;
            _parentList.Add(this);
            _priority = LastPriority++;
            Time = time;
            _type = type;
        }


        public int CompareTo(UpdateEvent other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;
            var output = _time.CompareTo(other._time);
            return output == 0 ? -_priority.CompareTo(other._priority) : output;
        }
    }
}