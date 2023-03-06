using System;
using UStacker.Gameplay.Communication;
using NLua;
using NLua.Exceptions;
using UStacker.Common.LuaApi;
using Random = UStacker.Common.Random;

namespace UStacker.Gameplay.GarbageGeneration
{
    public class CustomGarbageGenerator : IGarbageGenerator, IDisposable
    {

        private const string BOARD_VARIABLE_NAME = "Board";
        private readonly LuaFunction _generationFunction;
        private readonly LuaFunction _resetFunction;
        private readonly Random _random;
        private readonly Mediator _mediator;
        private readonly Lua _luaState;

        public CustomGarbageGenerator(GarbageBoardInterface boardInterface, string script, Mediator mediator)
        {
            _mediator = mediator;
            _luaState = CreateLua.WithAllPrerequisites(out _random);
            _luaState[BOARD_VARIABLE_NAME] = boardInterface;
            try
            {
                var scriptOutput = _luaState.DoString(script);

                if (scriptOutput.Length < 2)
                {
                    _mediator.Send(new GameCrashedMessage("Custom garbage script doesn't return 2 values"));
                    return;
                }

                _generationFunction = scriptOutput[0] as LuaFunction;
                _resetFunction = scriptOutput[1] as LuaFunction;
                if (_generationFunction is null)
                {
                    _mediator.Send(new GameCrashedMessage("Custom garbage generation function is not valid"));
                    return;
                }

                if (_resetFunction is null)
                    _mediator.Send(new GameCrashedMessage("Custom garbage generator reset function is not valid"));

            }
            catch (LuaException ex)
            {
                _mediator.Send(new GameCrashedMessage($"Custom garbage generator script crashed. Lua exception: {ex.Message}"));
            }
        }

        public void ResetState(ulong seed)
        {
            try
            {
                _resetFunction.Call();
            }
            catch (LuaException ex)
            {
                _mediator.Send(new GameCrashedMessage($"Custom garbage generator reset function crashed. Lua exception: {ex.Message}"));
            }
            _random.State = seed;
        }

        public void GenerateGarbage(uint amount, PiecePlacedMessage message)
        {
            try
            {
                _generationFunction.Call(amount, message);
            }
            catch (LuaException ex)
            {
                _mediator.Send(new GameCrashedMessage($"Custom garbage generation function crashed. Lua exception: {ex.Message}"));
            }
        }
        
        public void Dispose()
        {
            _generationFunction?.Dispose();
            _resetFunction?.Dispose();
            _luaState?.Dispose();
        }
    }
}