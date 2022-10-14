﻿using JetBrains.Annotations;

namespace Blockstacker.Gameplay.Communication
{
    public record GameEndConditionChangedMessage : MidgameMessage
    {
        [UsedImplicitly]
        public readonly double TotalCount;
        [UsedImplicitly]
        public readonly double CurrentCount;
        [UsedImplicitly]
        public readonly string ConditionName;

        public GameEndConditionChangedMessage(double time, double totalCount, double currentCount, string conditionName)
            : base(time)
        {
            TotalCount = totalCount;
            CurrentCount = currentCount;
            ConditionName = conditionName;
        }
    }
}