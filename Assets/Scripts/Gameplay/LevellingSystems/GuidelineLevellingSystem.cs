namespace Blockstacker.Gameplay.LevellingSystems
{
    public class GuidelineLevellingSystem : ILevellingSystem
    {
        public LevellingSystemInData InData { get; private set; }
        public LevellingSystemOutData OutData { get; private set; }


        public void Initialize(LevellingSystemInData inData, LevellingSystemOutData outData, uint startingLevel)
        {
            throw new System.NotImplementedException();
        }
    }
}