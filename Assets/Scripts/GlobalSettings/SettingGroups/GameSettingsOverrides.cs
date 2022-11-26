using System;
using UnityEngine;

namespace Blockstacker.GlobalSettings.Groups
{
    [Serializable]
    public record GameSettingsOverrides
    {
        [SerializeField]
        private float _countdownInterval = .5f;
        [SerializeField]
        private uint _countdownCount = 3;
        
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

        [field: SerializeField] public string StartingLevel { get; set; } = "1";
    }
}