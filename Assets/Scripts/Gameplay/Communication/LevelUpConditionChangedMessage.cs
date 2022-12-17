using JetBrains.Annotations;

namespace UStacker.Gameplay.Communication
{
    public record LevelUpConditionChangedMessage : MidgameMessage
    {
        [UsedImplicitly]
        public readonly string ConditionName;
        [UsedImplicitly]
        public readonly double CurrentCount;
        [UsedImplicitly]
        public readonly double TotalCount;

        public LevelUpConditionChangedMessage(double time, double totalCount, double currentCount, string conditionName) : base(time)
        {
            TotalCount = totalCount;
            CurrentCount = currentCount;
            ConditionName = conditionName;
        }
    }
}