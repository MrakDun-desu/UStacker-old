using NLua;

namespace Blockstacker.Gameplay.Levelling
{
    public class CustomLevellingSystem
    {
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

        private void DataUpdated()
        {
            throw new System.NotImplementedException();
        }
    }
}