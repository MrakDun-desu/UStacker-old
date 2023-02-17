using System;

namespace UStacker.Gameplay.Communication
{
    [Serializable]
    public readonly struct LevelChangedMessage : IMidgameMessage
    {
        public readonly string Level;
        public double Time { get; }

        public LevelChangedMessage(string level, double time)
        {
            Time = time;
            Level = level;
        }

    }
}