
/************************************
LevelChangedMessage.cs -- created by Marek Dančo (xdanco00)
*************************************/
using System;

namespace UStacker.Gameplay.Communication
{
    [Serializable]
    public readonly struct LevelChangedMessage : IMidgameMessage
    {
        public readonly string Level;

        public LevelChangedMessage(string level, double time)
        {
            Time = time;
            Level = level;
        }

        public double Time { get; }
    }
}
/************************************
end LevelChangedMessage.cs
*************************************/
