#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace FishNet
{
    internal static class ScriptingDefines
    {
        [InitializeOnLoadMethod]
        public static void AddDefineSymbols()
        {
            var currentDefines =
                PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
            /* Convert current defines into a hashset. This is so we can
             * determine if any of our defines were added. Only save playersettings
             * when a define is added. */
            var definesHs = new HashSet<string>();
            var currentArr = currentDefines.Split(';');
            //Add current defines into hs.
            foreach (var item in currentArr)
                definesHs.Add(item);

            var proDefine = "FISHNET_PRO";
            string[] fishNetDefines =
            {
                "FISHNET"
            };
            var modified = false;
            //Now add FN defines.
            foreach (var item in fishNetDefines)
                modified |= definesHs.Add(item);

            /* Remove pro define if not on pro. This might look a little
             * funny because the code below varies depending on if pro or not. */

#pragma warning disable CS0162 // Unreachable code detected
            modified |= definesHs.Remove(proDefine);
#pragma warning restore CS0162 // Unreachable code detected

            if (modified)
            {
                Debug.Log("Added Fish-Networking defines to player settings.");
                var changedDefines = string.Join(";", definesHs);
                PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup,
                    changedDefines);
            }
        }
    }
}
#endif