#if UNITY_EDITOR
using System.Collections.Generic;
using FishNet.Editing.PrefabCollectionGenerator;
using FishNet.Object;
using FishNet.Utility.Extension;
using FishNet.Utility.Performance;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FishNet.Editing
{
    public class ConfigurationEditor : EditorWindow
    {
        [MenuItem("Fish-Networking/Configuration", false, 0)]
        public static void ShowConfiguration()
        {
            SettingsService.OpenProjectSettings("Project/Fish-Networking/Configuration");
        }
    }

    public class RebuildSceneIdMenu : MonoBehaviour
    {
        /// <summary>
        ///     Rebuilds sceneIds for open scenes.
        /// </summary>
        [MenuItem("Fish-Networking/Rebuild SceneIds", false, 20)]
        public static void RebuildSceneIds()
        {
            var generatedCount = 0;
            for (var i = 0; i < SceneManager.sceneCount; i++)
            {
                var s = SceneManager.GetSceneAt(i);

                ListCache<NetworkObject> nobs;
                SceneFN.GetSceneNetworkObjects(s, false, out nobs);
                for (var z = 0; z < nobs.Written; z++)
                {
                    var nob = nobs.Collection[z];
                    nob.TryCreateSceneID();
                    EditorUtility.SetDirty(nob);
                }

                generatedCount += nobs.Written;

                ListCaches.StoreCache(nobs);
            }

            Debug.Log(
                $"Generated sceneIds for {generatedCount} objects over {SceneManager.sceneCount} scenes. Please save your open scenes.");
        }
    }

    public class RefreshDefaultPrefabsMenu : MonoBehaviour
    {
        /// <summary>
        ///     Rebuilds the DefaultPrefabsCollection file.
        /// </summary>
        [MenuItem("Fish-Networking/Refresh Default Prefabs", false, 22)]
        public static void RebuildDefaultPrefabs()
        {
            Debug.Log("Refreshing default prefabs.");
            Generator.GenerateFull(null, true);
        }
    }


    public class RemoveDuplicateNetworkObjectsMenu : MonoBehaviour
    {
        /// <summary>
        ///     Iterates all network object prefabs in the project and open scenes, removing NetworkObject components which exist
        ///     multiple times on a single object.
        /// </summary>
        [MenuItem("Fish-Networking/Remove Duplicate NetworkObjects", false, 21)]
        public static void RemoveDuplicateNetworkObjects()
        {
            var foundNobs = new List<NetworkObject>();

            foreach (var path in Generator.GetPrefabFiles("Assets", new HashSet<string>(), true))
            {
                var nob = AssetDatabase.LoadAssetAtPath<NetworkObject>(path);
                if (nob != null)
                    foundNobs.Add(nob);
            }

            //Now add scene objects.
            for (var i = 0; i < SceneManager.sceneCount; i++)
            {
                var s = SceneManager.GetSceneAt(i);

                ListCache<NetworkObject> nobs;
                SceneFN.GetSceneNetworkObjects(s, false, out nobs);
                for (var z = 0; z < nobs.Written; z++)
                {
                    var nob = nobs.Collection[z];
                    nob.TryCreateSceneID();
                    EditorUtility.SetDirty(nob);
                }

                for (var z = 0; z < nobs.Written; z++)
                    foundNobs.Add(nobs.Collection[i]);

                ListCaches.StoreCache(nobs);
            }

            //Remove duplicates.
            var removed = 0;
            foreach (var nob in foundNobs)
            {
                var count = nob.RemoveDuplicateNetworkObjects();
                if (count > 0)
                    removed += count;
            }

            Debug.Log($"Removed {removed} duplicate NetworkObjects. Please save your open scenes and project.");
        }
    }
}
#endif