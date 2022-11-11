using System;
using UnityEngine;

namespace Blockstacker.GameSettings.SettingGroups
{
    [Serializable]
    public record PresentationSettings
    {
        // backing fields
        [SerializeField]
        private float _countdownInterval = 1;
        [SerializeField]
        private uint _countdownCount = 3;

        [field: SerializeField]
        public string Title { get; set; } = "Custom game";
        [field: SerializeField]
        public bool UseCountdown { get; set; } = true;

        public float CountdownInterval
        {
            get => _countdownInterval;
            set => _countdownInterval = Mathf.Clamp(value, 0.1f, 10f);
        }

        public uint CountdownCount
        {
            get => _countdownCount;
            set => _countdownCount = Math.Min(value, 10);
        }
    }
}