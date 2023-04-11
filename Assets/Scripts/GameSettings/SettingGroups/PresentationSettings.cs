using System;
using Newtonsoft.Json;
using UnityEngine;
using UStacker.GlobalSettings.StatCounting;

namespace UStacker.GameSettings.SettingGroups
{
    [Serializable]
    public record PresentationSettings
    {
        // backing fields
        [SerializeField] private string _title = "Custom game";
        [SerializeField] private float _countdownInterval = .65f;
        [SerializeField] private uint _countdownCount = 3;
        [SerializeField] private uint _gamePadding = 3;

        public string Title
        {
            get => _title;
            set
            {
                _title = value.Length switch
                {
                    <= 0 => "Unset",
                    > 100 => value[..100],
                    _ => value
                };
            }
        }

        public float CountdownInterval
        {
            get => _countdownInterval;
            set => _countdownInterval = Mathf.Clamp(value, 0.1f, 10f);
        }

        public uint CountdownCount
        {
            get => _countdownCount;
            set => _countdownCount = Math.Clamp(value, 1, 10);
        }

        public uint GamePadding
        {
            get => _gamePadding;
            set => _gamePadding = Math.Min(value, 50);
        }

        [field: SerializeField] public StatCounterGroup DefaultStatCounterGroup { get; set; } = new();

        [JsonIgnore] public Guid? StatCounterGroupOverrideId { get; set; }
    }
}