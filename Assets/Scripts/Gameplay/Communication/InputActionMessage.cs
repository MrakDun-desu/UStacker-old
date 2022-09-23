using System;
using Blockstacker.Gameplay.Enums;

namespace Blockstacker.Gameplay.Communication
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