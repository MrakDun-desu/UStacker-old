using System;
using Blockstacker.Gameplay.Enums;

namespace Blockstacker.Gameplay.Communication
{
    [Serializable]
    public record InputActionMessage : MidgameMessage
    {
        public ActionType ActionType;
        public KeyActionType KeyActionType;
    }
}