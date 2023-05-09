
/************************************
LevelUpConditionChangedMessage.cs -- created by Marek Dančo (xdanco00)
*************************************/
using JetBrains.Annotations;

namespace UStacker.Gameplay.Communication
{
    public readonly struct LevelUpConditionChangedMessage : IMidgameMessage
    {
        [UsedImplicitly] public readonly string ConditionName;
        [UsedImplicitly] public readonly double CurrentCount;
        [UsedImplicitly] public readonly double TotalCount;

        public double Time { get; }

        public LevelUpConditionChangedMessage(double time, double totalCount, double currentCount, string conditionName)
        {
            Time = time;
            TotalCount = totalCount;
            CurrentCount = currentCount;
            ConditionName = conditionName;
        }
    }
}
/************************************
end LevelUpConditionChangedMessage.cs
*************************************/
