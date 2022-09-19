using System;
using System.Collections.Generic;

namespace Blockstacker.GlobalSettings.StatCounting
{
    [Serializable]
    public class StatCounterGroup
    {
        public string Name;
        public List<StatCounterRecord> StatCounters = new();
    }
}