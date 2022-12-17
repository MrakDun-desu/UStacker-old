using System.Collections.Generic;
using Blockstacker.Common;
using Blockstacker.Common.Extensions;
using NLua;
using NLua.Exceptions;

namespace Blockstacker.Gameplay.Randomizers
{
    public class CustomRandomizer : IRandomizer
    {
        private const string AVAILABLE_PIECES_VARIABLE_NAME = "AvailablePieces";
        private readonly List<string> _availableValues = new()
        {
            "i",
            "t",
            "o",
            "l",
            "j",
            "s",
            "z"
        };

        private readonly Lua _luaState = CreateLua.WithAllPrerequisites();
        private LuaFunction _nextPieceFunction;
        private LuaFunction _resetFunction;

        public CustomRandomizer(IEnumerable<string> availablePieces, string script, int seed,
            out string validationErrors)
        {
            _availableValues = _availableValues.Filter(availablePieces);
            validationErrors = ValidateScript(script);
            if (validationErrors is not null) return;

            _luaState = CreateLua.WithAllPrerequisites();
            InsertAvailableValuesIntoLua();
            var scriptOutput = _luaState.DoString(script);
            _nextPieceFunction = scriptOutput[0] as LuaFunction;
            _resetFunction = scriptOutput[1] as LuaFunction;
            if (_nextPieceFunction is null)
            {
                validationErrors = "Next piece function was not returned correctly.";
                return;
            }
            if (_resetFunction is null)
            {
                validationErrors = "Reset function was not returned correctly.";
                return;
            }
            Reset(seed);
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

        public void Reset(int newSeed)
        {
            _resetFunction.Call(newSeed);
        }

        private void InsertAvailableValuesIntoLua()
        {
            _luaState.DoString($"{AVAILABLE_PIECES_VARIABLE_NAME} = {{}}");
            var availablePiecesTable = (LuaTable) _luaState[AVAILABLE_PIECES_VARIABLE_NAME];
            for (var i = 0; i < _availableValues.Count; i++)
                availablePiecesTable[i + 1] = _availableValues[i];
        }

        private string ValidateScript(string script)
        {
            try
            {
                InsertAvailableValuesIntoLua();
                var retValue = _luaState.DoString(script);
                if (retValue is null || retValue.Length < 2)
                    return "Error: Randomizer script doesn't return 2 values";
                _nextPieceFunction = retValue[0] as LuaFunction;
                _resetFunction = retValue[1] as LuaFunction;
                if (_nextPieceFunction is null)
                    return "Error: Randomizer script doesn't return next piece function correctly";
                if (_resetFunction is null)
                    return "Error: Randomizer script doesn't return reset function correctly";
                _resetFunction.Call(42);
            }
            catch (LuaException ex)
            {
                return $"Error executing randomizer script: {ex.Message}";
            }

            var result = _nextPieceFunction?.Call();
            if (result is null || result.Length < 1)
                return "Error: next piece function doesn't return";

            if (result[0] is not string nextPiece)
                return $"Error: next piece function doesn't return a valid value. Returned value: {result[0]}";

            return _availableValues.Contains(nextPiece)
                ? null
                : $"Error: next piece function returns a non-existent piece type. Returned value: {result[0]}";
        }
    }
}