using System;
using NLua;

namespace Blockstacker.Gameplay.Levelling
{
    public class CustomLevellingSystem
    {
        private string _customScript;
        private Lua _luaState = new();

        public CustomLevellingSystem(string levellingSystemScript, out bool isValid)
        {
            _customScript = levellingSystemScript;
            isValid = ValidateScript();
        }

        private bool ValidateScript()
        {
            throw new NotImplementedException();
        }

        private void DataUpdated()
        {
            throw new NotImplementedException();
        }
    }
}