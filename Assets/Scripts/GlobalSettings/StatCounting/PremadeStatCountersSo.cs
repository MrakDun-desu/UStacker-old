using System;
using UnityEngine;

namespace UStacker.GlobalSettings.StatCounting
{
    [CreateAssetMenu(fileName = "PremadeStatCounters", menuName = "UStacker/Premade stat counters", order = 0)]
    public class PremadeStatCountersSo : ScriptableObject
    {
        public StatCounterGroup[] PremadeGroups = Array.Empty<StatCounterGroup>();
        public StatCounterGroup DefaultGroup = new();
    }
}