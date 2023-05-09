
/************************************
StatCounterSO.cs -- created by Marek Dančo (xdanco00)
*************************************/
using UnityEngine;

namespace UStacker.GlobalSettings.StatCounting
{
    [CreateAssetMenu(fileName = "StatCounter", menuName = "UStacker/Stat counter")]
    public class StatCounterSO : ScriptableObject
    {
        public StatCounterRecord Value;
    }
}
/************************************
end StatCounterSO.cs
*************************************/
