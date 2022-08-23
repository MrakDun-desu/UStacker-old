using NLua;
using NLua.Exceptions;
using UnityEngine;

namespace Blockstacker.Gameplay.Randomizers
{
    public class CustomRandomizer : IRandomizer
    {
        private readonly Lua _luaState = new();
        private readonly int _pieceCount; // not needed for now
        private LuaFunction _nextPieceFunction;
        private const string SEED_VARIABLE_NAME = "Seed";

        public CustomRandomizer(int pieceCount, string script, int seed, out string validationErrors)
        {
            _pieceCount = pieceCount;
            validationErrors = ValidateScript(seed, script);
            if (validationErrors is not null) return;
            _luaState = new Lua();
            _luaState[SEED_VARIABLE_NAME] = seed;
            _nextPieceFunction = _luaState.DoString(script)[0] as LuaFunction;
        }

        public int GetNextPiece()
        {
            var result = _nextPieceFunction.Call();

            if (result.Length < 1) return 0;

            var nextPiece = result[0] switch
            {
                double d => (int) d,
                long l => (int) l,
                int i => i,
                _ => 0
            };

            nextPiece = Mathf.Clamp(nextPiece, 0, _pieceCount - 1);
            return nextPiece;
        }

        private string ValidateScript(int seed, string script)
        {
            try
            {
                _luaState[SEED_VARIABLE_NAME] = seed;
                var retValue = _luaState.DoString(script);
                if (retValue.Length < 1) 
                    return "Error: Randomizer script doesn't return.";
                _nextPieceFunction = retValue[0] as LuaFunction;
            }
            catch (LuaException ex)
            {
                return $"Error executing randomizer script: {ex.Message}";
            }

            var result = _nextPieceFunction?.Call();
            if (result is null || result.Length < 1) 
                return "Error: Randomizer script doesn't return a valid type";

            var nextPiece = result[0] switch
            {
                double d => (int) d,
                long l => (int) l,
                int i => i,
                _ => -1
            };

            return nextPiece >= 0 && nextPiece <= _pieceCount - 1
                ? null
                : $"Error: Randomizer script doesn't return a valid value. Returned value: {nextPiece}";
        }
    }
}