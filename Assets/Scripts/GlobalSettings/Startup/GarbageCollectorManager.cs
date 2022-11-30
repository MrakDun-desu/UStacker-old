using Blockstacker.Common;
using UnityEngine.SceneManagement;
using UnityEngine.Scripting;

namespace Blockstacker.GlobalSettings.Startup
{
    public class GarbageCollectorManager : MonoSingleton<GarbageCollectorManager>
    {
        private const string GAME_STRING = "Scene_Game";

        protected override void Awake()
        {
            base.Awake();
            SceneManager.sceneLoaded += OnSceneChanged;
        }

        private static void OnSceneChanged(Scene scene, LoadSceneMode loadSceneMode)
        {
            if (loadSceneMode == LoadSceneMode.Additive) return;

            GarbageCollector.GCMode = scene.name.StartsWith(GAME_STRING)
                ? GarbageCollector.Mode.Disabled
                : GarbageCollector.Mode.Enabled;
        }
    }
}