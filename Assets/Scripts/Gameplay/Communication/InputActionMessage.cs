using System;
using UStacker.Gameplay.Enums;

namespace UStacker.Gameplay.Communication
{
    [Serializable]
    public readonly struct InputActionMessage : IMidgameMessage
    {
        public readonly ActionType ActionType;
        public readonly KeyActionType KeyActionType;
        public double Time { get; }

        public InputActionMessage(ActionType actionType, KeyActionType keyActionType, double time)
        {
            Time = time;
            ActionType = actionType;
            KeyActionType = keyActionType;
        }
    }
}