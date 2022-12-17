using System;
using UStacker.Gameplay.Enums;

namespace UStacker.Gameplay.Communication
{
    [Serializable]
    public record InputActionMessage : MidgameMessage
    {
        public readonly ActionType ActionType;
        public readonly KeyActionType KeyActionType;

        public InputActionMessage(ActionType actionType, KeyActionType keyActionType, double time) : base(time)
        {
            ActionType = actionType;
            KeyActionType = keyActionType;
        }
    }
}