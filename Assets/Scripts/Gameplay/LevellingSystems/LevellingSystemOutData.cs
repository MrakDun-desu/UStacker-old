using System;

namespace Blockstacker.Gameplay.LevellingSystems
{
    public class LevellingSystemOutData
    {
        public float gravity;
        public float lockDelay;
        public uint level;
        public long score;

        public void SetValues(float gravity, float lockDelay, uint level, long score)
        {
            this.gravity = gravity;
            this.lockDelay = lockDelay;
            this.level = level;
            this.score = score;
            Changed?.Invoke();
        }

        public event Action Changed;
    }
}