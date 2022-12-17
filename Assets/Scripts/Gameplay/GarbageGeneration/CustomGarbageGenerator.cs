using Blockstacker.Common;
using Blockstacker.Gameplay.Communication;
using NLua;
using NLua.Exceptions;

namespace Blockstacker.Gameplay.GarbageGeneration
{
    public class CustomGarbageGenerator : IGarbageGenerator
    {

        private const string BOARD_VARIABLE_NAME = "Board";
        private readonly Lua _luaState = CreateLua.WithAllPrerequisites();
        private LuaFunction _generationFunction;
        private LuaFunction _resetFunction;

        public CustomGarbageGenerator(GarbageBoardInterface boardInterface, string script, out string validationErrors)
        {
            validationErrors = ValidateScript(script);
            if (validationErrors != null)
                return;

            _luaState = CreateLua.WithAllPrerequisites();
            _luaState[BOARD_VARIABLE_NAME] = boardInterface;
            var scriptOutput = _luaState.DoString(script);
            _generationFunction = scriptOutput[0] as LuaFunction;
            _resetFunction = scriptOutput[1] as LuaFunction;
            if (_generationFunction is null)
            {
                validationErrors = "Generation function was not returned correctly.";
                return;
            }

            if (_resetFunction is not null) return;
            validationErrors = "Reset function was not returned correctly.";
        }

        public void ResetState(int seed)
        {
            _resetFunction.Call(seed);
        }

        public void GenerateGarbage(uint amount, PiecePlacedMessage message)
        {
            _generationFunction.Call(amount, message);
        }

        private string ValidateScript(string script)
        {
            try
            {
                _luaState[BOARD_VARIABLE_NAME] = new GarbageBoardInterface(null);

                var result = _luaState.DoString(script);
                if (result.Length < 2)
                    return "Error: Garbage generator script doesn't return 2 values";

                _generationFunction = result[0] as LuaFunction;
                _resetFunction = result[0] as LuaFunction;
                if (_generationFunction is null)
                    return "Error: Garbage generator script doesn't return generation function correctly";
                if (_resetFunction is null)
                    return "Error: Garbage generator script doesn't return reset function correctly";

                _resetFunction.Call(42);

                GenerateGarbage(5, new PiecePlacedMessage());
                GenerateGarbage(5, null);
            }
            catch (LuaException ex)
            {
                return $"Error executing garbage generator script: {ex.Message}";
            }

            return null;
        }
    }
}