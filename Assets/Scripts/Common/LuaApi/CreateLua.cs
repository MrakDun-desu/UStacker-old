using NLua;

namespace UStacker.Common.LuaApi
{
    public static class CreateLua
    {
        public static Lua WithAllPrerequisites(out Random random)
        {
            var output = new Lua()
                .AddRestrictions()
                .AddCustomRandom(out random)
                .AddDebug();
            return output;
        }

        public static Lua AddCustomRandom(this Lua output, out Random random)
        {
            random = new Random();
            output.RegisterFunction("math.random", random,
                typeof(Random).GetMethod(nameof(random.NextLua)));
            return output;
        }

        public static Lua AddRestrictions(this Lua output)
        {
            output["os"] = null;
            output["io"] = null;
            output["require"] = null;
            output["dofile"] = null;
            output["package"] = null;
            output["luanet"] = null;
            output["load"] = null;
            return output;
        }

        public static Lua AddDebug(this Lua output)
        {
            output.RegisterFunction("DebugLog", typeof(Logger).GetMethod(nameof(Logger.Log)));
            return output;
        }
    }
}