using UnityEngine;

namespace Blockstacker.GlobalSettings.StatCounting
{
    [CreateAssetMenu(fileName = "StatCounter", menuName = "Blockstacker/Stat counter")]
    public class StatCounterSO : ScriptableObject
    {
        public StatCounterRecord Value;
    }
}