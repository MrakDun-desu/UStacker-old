namespace Blockstacker.Gameplay.Recording
{
    public struct HoldRecord : IRecord
    {
        public float Time { get; }

        public HoldRecord(float time)
        {
            Time = time;
        }
    }
}