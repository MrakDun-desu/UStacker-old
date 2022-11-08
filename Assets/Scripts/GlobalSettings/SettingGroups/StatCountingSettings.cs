﻿using System;
using System.Collections.Generic;
using Blockstacker.GlobalSettings.StatCounting;

namespace Blockstacker.GlobalSettings.Groups
{
    [Serializable]
    public record StatCountingSettings
    {
        public Dictionary<Guid, StatCounterGroup> StatCounterGroups { get; set; } = new();

        // not viewed in the global settings UI
        public Dictionary<string, Guid> GameStatCounterDictionary { get; set; } = new();
    }
}