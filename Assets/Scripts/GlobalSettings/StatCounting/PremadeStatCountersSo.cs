using System;
using UnityEngine;

namespace Blockstacker.GlobalSettings.StatCounting
{
    [CreateAssetMenu(fileName = "PremadeStatCounters", menuName = "Blockstacker/Premade stat counters", order = 0)]
    public class PremadeStatCountersSo : ScriptableObject
    {
        public StatCounterGroup[] PremadeGroups = Array.Empty<StatCounterGroup>();
    }
}