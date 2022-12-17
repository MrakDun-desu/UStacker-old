using NLua;
using UnityEngine;

namespace UStacker.Common
{
    public static class CreateLua
    {
        public static Lua WithAllPrerequisites()
        {
            var output = new Lua();
            output["os"] = null;
            output["io"] = null;
            output["require"] = null;
            output["dofile"] = null;
            output["package"] = null;
            output["luanet"] = null;
            output["load"] = null;
            output.RegisterFunction(nameof(DebugLog), typeof(CreateLua).GetMethod(nameof(DebugLog)));
            return output;
        }

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

        public static Lua WithDebug()
        {
            var output = new Lua();
            output.RegisterFunction(nameof(DebugLog), typeof(CreateLua).GetMethod(nameof(DebugLog)));
            return output;
        }

        public static void DebugLog(object obj)
        {
#if UNITY_EDITOR
            Debug.Log(obj);
#else
            File.AppendAllText(PersistentPaths.DebugPath, $"[{DateTime.Now}] {obj}\n");
#endif
        }
    }
}