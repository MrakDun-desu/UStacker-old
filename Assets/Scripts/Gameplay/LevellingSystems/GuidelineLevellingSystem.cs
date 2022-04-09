namespace Blockstacker.Gameplay.LevellingSystems
{
    public class GuidelineLevellingSystem : ILevellingSystem
    {
        public LevellingSystemOutData OutData { get; private set; }

        public LevellingSystemInData InData { get; private set; }

        public void Initialize(LevellingSystemInData inData, LevellingSystemOutData outData)
        {
            InData = inData;
            OutData = outData;
        }
    }
}