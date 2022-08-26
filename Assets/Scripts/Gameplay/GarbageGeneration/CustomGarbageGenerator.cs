using Blockstacker.Common.Extensions;
using Blockstacker.Gameplay.Communication;
using NLua;
using NLua.Exceptions;

namespace Blockstacker.Gameplay.GarbageGeneration
{
    public class CustomGarbageGenerator : IGarbageGenerator
    {
        private Lua _luaState;
        private readonly ReadonlyBoard _board;
        private readonly string _script;

        private LuaFunction _generationFunction;

        private const string BOARD_VARIABLE_NAME = "Board";
        private const string SEED_VARIABLE_NAME = "Seed";
        
        public CustomGarbageGenerator(ReadonlyBoard board, string script, out string validationErrors)
        {
            if (!ValidateScript(script, out validationErrors))
                return;

            _script = script;
            _board = board;
        }

        public void ResetState(int seed)
        {
            _luaState = new Lua();
            _luaState.RestrictMaliciousFunctions();
            _luaState[SEED_VARIABLE_NAME] = seed;
            _luaState[BOARD_VARIABLE_NAME] = _board;
            _generationFunction =  _luaState.DoString(_script)[0] as LuaFunction;
        }

        public void GenerateGarbage(uint amount, PiecePlacedMessage message)
        {
            _generationFunction.Call(amount, message);
        }

        private bool ValidateScript(string script, out string validationErrors)
        {
            try
            {
                _luaState = new Lua();
                _luaState[SEED_VARIABLE_NAME] = 0;
                _luaState[BOARD_VARIABLE_NAME] = new ReadonlyBoard(null);

                var result = _luaState.DoString(script);
                if (result.Length < 1)
                {
                    validationErrors = "Error: Garbage generator script doesn't return";
                    return false;
                }

                _generationFunction = result[0] as LuaFunction;
                if (_generationFunction is null)
                {
                    validationErrors = "Error: Garbage generator script doesn't return a function";
                    return false;
                }

                GenerateGarbage(5, new PiecePlacedMessage());
                GenerateGarbage(5, null);
            }
            catch (LuaException ex)
            {
                validationErrors = $"Error executing garbage generator script: {ex.Message}";
                return false;
            }

            validationErrors = null;
            return true;
        }
    }
}