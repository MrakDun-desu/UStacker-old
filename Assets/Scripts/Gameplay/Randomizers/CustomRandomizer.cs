using System.Collections.Generic;
using System.Linq;
using Blockstacker.Common.Extensions;
using NLua;
using NLua.Exceptions;
using UnityEngine;

namespace Blockstacker.Gameplay.Randomizers
{
    public class CustomRandomizer : IRandomizer
    {
        private readonly Lua _luaState = new();
        private readonly string[] _availableValues;
        private LuaFunction _nextPieceFunction;
        private const string SEED_VARIABLE_NAME = "Seed";
        private const string AVAILABLE_PIECES_VARIABLE_NAME = "AvailablePieces";

        public CustomRandomizer(IEnumerable<string> availablePieces, string script, int seed,
            out string validationErrors)
        {
            _availableValues = availablePieces.ToArray();
            validationErrors = ValidateScript(seed, script);
            if (validationErrors is not null) return;
            _luaState = new Lua();
            _luaState[SEED_VARIABLE_NAME] = seed;
            _nextPieceFunction = _luaState.DoString(script)[0] as LuaFunction;
        }

        public string GetNextPiece()
        {
            try
            {
                var result = _nextPieceFunction.Call();

                if (result.Length < 1) return "";

                if (result[0] is not string nextPiece)
                    return "";

                return _availableValues.Contains(nextPiece)
                    ? nextPiece
                    : "";
            }
            catch (LuaException)
            {
                return "";
            }
        }

        private string ValidateScript(int seed, string script)
        {
            _luaState.RestrictMaliciousFunctions();
            try
            {
                _luaState[AVAILABLE_PIECES_VARIABLE_NAME] = _availableValues;
                _luaState[SEED_VARIABLE_NAME] = seed;
                var retValue = _luaState.DoString(script);
                if (retValue.Length < 1)
                    return "Error: Randomizer script doesn't return";
                _nextPieceFunction = retValue[0] as LuaFunction;
                if (_nextPieceFunction is null)
                    return "Error: Randomizer script doesn't return a function";
            }
            catch (LuaException ex)
            {
                return $"Error executing randomizer script: {ex.Message}";
            }

            var result = _nextPieceFunction?.Call();
            if (result is null || result.Length < 1)
                return "Error: Randomizer function doesn't return";

            if (result[0] is not string nextPiece)
                return $"Error: Randomizer function doesn't return a valid value. Returned value: {result[0]}";
            
            return _availableValues.Contains(nextPiece)
                ? null
                : $"Error: Randomizer function returns a non-existent piece type. Returned value: {result[0]}";
        }
    }
}