using System;
using System.Collections.Generic;
using UStacker.GlobalSettings.StatCounting;

namespace UStacker.GlobalSettings.Groups
{
    [Serializable]
    public record StatCountingSettings
    {
        public Dictionary<Guid, StatCounterGroup> StatCounterGroups { get; set; } = new();
    }
}