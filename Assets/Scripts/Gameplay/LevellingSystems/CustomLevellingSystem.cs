using NLua;

namespace Blockstacker.Gameplay.LevellingSystems
{
    public class CustomLevellingSystem : ILevellingSystem
    {
        public LevellingSystemInData InData { get; private set; }
        public LevellingSystemOutData OutData { get; private set; }

        private Lua _luaState = new();
        private string _customScript;

        private bool ValidateScript()
        {
            throw new System.NotImplementedException();
        }

        public CustomLevellingSystem(string levellingSystemScript, out bool isValid)
        {
            _customScript = levellingSystemScript;
            isValid = ValidateScript();
        }

        public void Initialize(LevellingSystemInData inData, LevellingSystemOutData outData, uint startingLevel)
        {
            InData = inData;
            OutData = outData;
            InData.Changed += DataUpdated;
        }

        private void DataUpdated()
        {
            throw new System.NotImplementedException();
        }
    }
}