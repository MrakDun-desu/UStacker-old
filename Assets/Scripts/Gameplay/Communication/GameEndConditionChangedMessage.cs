using JetBrains.Annotations;

namespace UStacker.Gameplay.Communication
{
    public readonly struct GameEndConditionChangedMessage : IMidgameMessage
    {
        [UsedImplicitly]
        public readonly string ConditionName;
        [UsedImplicitly]
        public readonly double CurrentCount;
        [UsedImplicitly]
        public readonly double TotalCount;
        public double Time { get; }

        public GameEndConditionChangedMessage(double time, double totalCount, double currentCount, string conditionName)
        {
            Time = time;
            TotalCount = totalCount;
            CurrentCount = currentCount;
            ConditionName = conditionName;
        }
    }
}