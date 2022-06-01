using System;
using Blockstacker.Gameplay.Enums;

namespace Blockstacker.Gameplay.Communication
{
    [Serializable]
    public record InputActionMessage : Message
    {
        public ActionType ActionType;
        public KeyActionType KeyActionType;
    }
}