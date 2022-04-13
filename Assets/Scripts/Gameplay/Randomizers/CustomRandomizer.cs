using NLua;
using NLua.Exceptions;
using UnityEngine;

namespace Blockstacker.Gameplay.Randomizers
{
    public class CustomRandomizer : IRandomizer
    {
        private readonly string _script;
        private readonly int _pieceCount;
        private readonly Lua _luaState = new();

        private bool ValidateScript(int seed)
        {
            try {
                _luaState.DoString(_script);
            }
            catch (LuaScriptException) {
                return false;
            }
            var nextPiece = _luaState["nextPiece"];
            if (nextPiece is null) return false;
            if (nextPiece.GetType() != typeof(double) &&
                nextPiece.GetType() != typeof(long) &&
                nextPiece.GetType() != typeof(int))
                return false;

            _luaState.DoString($"seed = {seed}");
            _luaState.DoString("newlySetSeed = true");
            return true;
        }

        public CustomRandomizer(int pieceCount, string script, int seed, out bool isValid)
        {
            _script = script;
            _pieceCount = pieceCount;
            _luaState.DoString($"pieceCount = {pieceCount}");
            _luaState.DoString($"seed = {seed}");
            _luaState.DoString("newlySetSeed = true");
            isValid = ValidateScript(seed);
        }

        public int GetNextPiece()
        {
            _luaState.DoString(_script);
            var result = _luaState["nextPiece"];

            var nextPiece = result switch
            {
                double d => (int)d,
                long l => (int)l,
                int i => i,
                _ => 0
            };

            nextPiece = Mathf.Clamp(nextPiece, 0, _pieceCount - 1);
            return nextPiece;
        }
    }
}