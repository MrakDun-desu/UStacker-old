using System;
using System.Collections.Generic;

namespace Blockstacker.GlobalSettings.StatCounting
{
    [Serializable]
    public class StatCounterGroup
    {
        public string Name;
        public List<StatCounterRecord> StatCounters = new();

        public StatCounterGroup Copy()
        {
            var output = new StatCounterGroup {Name = Name};

            foreach (var counter in StatCounters)
            {
                output.StatCounters.Add(counter.Copy());
            }

            return output;
        }
    }
}