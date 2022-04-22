using NLua;
using NLua.Exceptions;
using UnityEngine;

namespace Blockstacker.Gameplay.Randomizers
{
    public class CustomRandomizer : IRandomizer
    {
        private readonly Lua _luaState = new();
        private readonly int _pieceCount; // not needed for now - 

        private bool ValidateScript(int seed, string script)
        {
            try
            {
                _luaState.DoString($"seed = {seed}");
                _luaState.DoString(script);
            }
            catch (LuaScriptException) {
                return false;
            }
            var result = _luaState.DoString("GetNextPiece()");
            if (result is null || result.Length < 1) return false;

            var nextPiece = result[0] switch
            {
                double d => (int)d,
                long l => (int)l,
                int i => i,
                _ => -1
            };

            return nextPiece >= 0 && nextPiece <= _pieceCount - 1;
        }

        public CustomRandomizer(int pieceCount, string script, int seed, out bool isValid)
        {
            _pieceCount = pieceCount;
            isValid = ValidateScript(seed, script);
            if (!isValid) return;
            _luaState = new Lua();
            _luaState.DoString($"seed = {seed}");
            _luaState.DoString(script);
        }

        public int GetNextPiece()
        {
            var result = _luaState.DoString("GetNextPiece()");

            if (result.Length < 1) return 0;

            var nextPiece = result[0] switch
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