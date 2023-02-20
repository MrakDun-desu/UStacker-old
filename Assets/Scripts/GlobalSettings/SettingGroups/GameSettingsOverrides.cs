using System;
using JetBrains.Annotations;
using UnityEngine;

namespace UStacker.GlobalSettings.Groups
{
    [Serializable]
    public record GameSettingsOverrides
    {
        private uint? _countdownCount;
        private float? _countdownInterval;
        [field: SerializeField] [CanBeNull] public string StartingLevel { get; set; }

        public float? CountdownInterval
        {
            get => _countdownInterval;
            set
            {
                if (value is { } floatVal)
                {
                    _countdownInterval = Mathf.Clamp(floatVal, 0.1f, 10f);
                    return;
                }

                _countdownInterval = null;
            }
        }

        public uint? CountdownCount
        {
            get => _countdownCount;
            set
            {
                if (value is { } uintVal)
                {
                    _countdownCount = Math.Clamp(uintVal, 2u, 10u);
                    return;
                }

                _countdownCount = null;
            }
        }
    }
}