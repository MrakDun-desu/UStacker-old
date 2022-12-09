using Blockstacker.Common.Extensions;
using NLua;

namespace Blockstacker.Common
{
    public static class CreateLua
    {
        public static Lua WithRestrictions()
        {
            var output = new Lua();
            output["os"] = null;
            output["io"] = null;
            output["require"] = null;
            output["dofile"] = null;
            output["package"] = null;
            output["luanet"] = null;
            output["load"] = null;
            return output;
        }
    }
}