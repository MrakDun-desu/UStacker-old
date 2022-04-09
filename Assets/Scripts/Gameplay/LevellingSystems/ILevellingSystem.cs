namespace Blockstacker.Gameplay.LevellingSystems
{
    public interface ILevellingSystem
    {
        LevellingSystemOutData OutData { get; }
        LevellingSystemInData InData { get; }
        void Initialize(LevellingSystemInData inData, LevellingSystemOutData outData, uint startingLevel);

    }
}