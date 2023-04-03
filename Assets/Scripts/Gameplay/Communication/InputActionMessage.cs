using System;
using UStacker.Gameplay.Enums;

namespace UStacker.Gameplay.Communication
{
    [Serializable]
    public readonly struct InputActionMessage : IMidgameMessage, IEquatable<InputActionMessage>
    {
        public bool Equals(InputActionMessage other)
        {
            return ActionType == other.ActionType && KeyActionType == other.KeyActionType && time.Equals(other.time);
        }

        public override bool Equals(object obj)
        {
            return obj is InputActionMessage other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine((int) ActionType, (int) KeyActionType, time);
        }

        public readonly ActionType ActionType;
        public readonly KeyActionType KeyActionType;
        public readonly double time;
        public double Time => time;

        public InputActionMessage(ActionType actionType, KeyActionType keyActionType, double time)
        {
            ActionType = actionType;
            KeyActionType = keyActionType;
            this.time = time;
        }
    }
}