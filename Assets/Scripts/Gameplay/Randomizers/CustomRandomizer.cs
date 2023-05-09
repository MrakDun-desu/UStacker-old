
/************************************
CustomRandomizer.cs -- created by Marek Dančo (xdanco00)
*************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using NLua;
using NLua.Exceptions;
using UStacker.Common.Extensions;
using UStacker.Common.LuaApi;
using UStacker.Gameplay.Communication;
using Random = UStacker.Common.Random;

namespace UStacker.Gameplay.Randomizers
{
    public class CustomRandomizer : IRandomizer, IDisposable
    {
        private const string AVAILABLE_PIECES_VARIABLE_NAME = "AvailablePieces";

        private readonly string[] _availableValues =
        {
            "i",
            "t",
            "o",
            "l",
            "j",
            "s",
            "z"
        };

        private readonly Lua _luaState;
        private readonly Mediator _mediator;
        private readonly LuaFunction _nextPieceFunction;
        private readonly Random _random;
        private readonly LuaFunction _resetFunction;

        public CustomRandomizer(IEnumerable<string> availablePieces, string script, Mediator mediator)
        {
            _luaState = CreateLua.WithAllPrerequisites(out _random);
            _availableValues = _availableValues.Filter(availablePieces).ToArray();
            _mediator = mediator;

            InsertAvailableValuesIntoLua();
            try
            {
                var scriptOutput = _luaState.DoString(script);

                if (scriptOutput.Length < 2)
                {
                    _mediator.Send(new GameCrashedMessage("Custom randomizer script doesn't return 2 values"));
                    return;
                }

                _nextPieceFunction = scriptOutput[0] as LuaFunction;
                _resetFunction = scriptOutput[1] as LuaFunction;
                if (_nextPieceFunction is null)
                {
                    _mediator.Send(new GameCrashedMessage("Custom randomizer next piece function is not valid"));
                    return;
                }

                if (_resetFunction is null)
                    _mediator.Send(new GameCrashedMessage("Custom randomizer reset function is not valid"));
            }
            catch (LuaException ex)
            {
                _mediator.Send(new GameCrashedMessage($"Custom randomizer crashed. Lua exception: {ex.Message}"));
            }
        }

        public void Dispose()
        {
            _luaState?.Dispose();
            _nextPieceFunction?.Dispose();
            _resetFunction?.Dispose();
        }

        public string GetNextPiece()
        {
            try
            {
                var result = _nextPieceFunction.Call();

                if (result.Length < 1)
                {
                    _mediator.Send(
                        new GameCrashedMessage("Custom randomizer next piece function didn't return a value"));
                    return null;
                }

                if (result[0] is not string nextPiece)
                {
                    _mediator.Send(new GameCrashedMessage(
                        $"Custom randomizer next piece function returned an invalid value \"{result[0]}\""));
                    return null;
                }

                if (_availableValues.Contains(nextPiece))
                    return nextPiece;

                _mediator.Send(new GameCrashedMessage(
                    $"Custom randomizer next piece function returned an invalid value \"{result[0]}\""));
                return null;
            }
            catch (LuaException ex)
            {
                _mediator.Send(
                    new GameCrashedMessage(
                        $"Custom randomizer next piece function crashed. Lua exception: {ex.Message}"));
                return null;
            }
        }

        public void Reset(ulong newSeed)
        {
            try
            {
                _resetFunction.Call();
            }
            catch (LuaException ex)
            {
                _mediator.Send(
                    new GameCrashedMessage($"Custom randomizer reset function crashed. Lua exception: {ex.Message}"));
            }

            _random.State = newSeed;
        }

        private void InsertAvailableValuesIntoLua()
        {
            _luaState.DoString($"{AVAILABLE_PIECES_VARIABLE_NAME} = {{}}");
            var availablePiecesTable = (LuaTable) _luaState[AVAILABLE_PIECES_VARIABLE_NAME];
            for (var i = 0; i < _availableValues.Length; i++)
                availablePiecesTable[i + 1] = _availableValues[i];
        }
    }
}
/************************************
end CustomRandomizer.cs
*************************************/
