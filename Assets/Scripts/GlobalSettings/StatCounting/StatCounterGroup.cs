using System.Collections.Generic;

namespace Blockstacker.GlobalSettings.StatCounting
{
    public class StatCounterGroup
    {
        public string Name;
        public StatCounterRecord LevellingStatCounter;
        public List<StatCounterRecord> StatCounters;
    }
}