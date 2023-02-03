using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace UStacker.Common
{
    public static class PersistentPaths
    {
        public static string GlobalSettings => Path.Combine(DataPath, "appSettings.json");

        public static string DebugPath => Path.Combine(DataPath, "logs.txt");
        public static string InputPresets => Path.Combine(DataPath, "inputPresets"); // currently unused
        public static string GameSettingsPresets => Path.Combine(DataPath, "gameSettingsPresets");
        public static string StatCounters => Path.Combine(DataPath, "statCounters");
        public static string Replays => Path.Combine(DataPath, "replays");

        private static string RulesCustomization => Path.Combine(DataPath, "rulesCustomization");
        public static string Randomizers => Path.Combine(RulesCustomization, "randomizers");
        public static string GarbageGenerators => Path.Combine(RulesCustomization, "garbageGenerators");
        public static string GameManagers => Path.Combine(RulesCustomization, "gameManagers");
        public static string RotationSystems => Path.Combine(RulesCustomization, "rotationSystems");

        private static string StyleCustomization => Path.Combine(DataPath, "styleCustomization");
        public static string BackgroundPacks => Path.Combine(StyleCustomization, "backgroundPacks");
        public static string Skins => Path.Combine(StyleCustomization, "skins");

        public static string SoundPacks => Path.Combine(StyleCustomization, "soundPacks");

        private const string DATA_PATH_KEY = "DataPath";
        public static string DataPath
        {
            get => PlayerPrefs.HasKey(DATA_PATH_KEY)
                    ? PlayerPrefs.GetString(DATA_PATH_KEY)
                    : Path.Combine(Application.persistentDataPath, "ApplicationData");
            private set => PlayerPrefs.SetString(DATA_PATH_KEY, value);
        }

        public static async Task<bool> TrySetDataPathAsync(string newPath)
        {
            var currentDataPathInfo = new DirectoryInfo(DataPath);
            var dirs = currentDataPathInfo.GetDirectories();
            var files = currentDataPathInfo.GetFiles();
                
            if (!FileHandling.CreateDirectoriesRecursively(newPath)) 
                return false;

            if (!await FileHandling.CopyDirectoryRecursivelyAsync(dirs, files, newPath))
                return false;

            DataPath = newPath;
            return true;
        }

    }
}