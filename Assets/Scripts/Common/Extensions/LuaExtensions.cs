using NLua;

namespace Blockstacker.Common.Extensions
{
    public static class LuaExtensions
    {
        public static void RestrictMaliciousFunctions(this Lua lua)
        {
            lua["os"] = null;
            lua["io"] = null;
            lua["require"] = null;
            lua["dofile"] = null;
            lua["package"] = null;
            lua["luanet"] = null;
            lua["load"] = null;
        }
    }
}