﻿namespace UStacker.Gameplay.Communication
{
    public readonly struct ScoreChangedMessage : IMidgameMessage
    {
        public readonly long Score;
        public double Time { get; }

        public ScoreChangedMessage(long score, double time)
        {
            Time = time;
            Score = score;
        }
    }
}