
/************************************
GameSettingsOverrides.cs -- created by Marek Dančo (xdanco00)
*************************************/
using System;
using JetBrains.Annotations;
using UnityEngine;

namespace UStacker.GlobalSettings.Groups
{
    [Serializable]
    public record GameSettingsOverrides
    {
        [field: SerializeField] [CanBeNull] public string StartingLevel { get; set; }
        private uint? _countdownCount;
        private float? _countdownInterval;

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
                    _countdownCount = Math.Clamp(uintVal, 1u, 10u);
                    return;
                }

                _countdownCount = null;
            }
        }

        public Guid? StatCounterGroupId { get; set; }
    }
}
/************************************
end GameSettingsOverrides.cs
*************************************/
