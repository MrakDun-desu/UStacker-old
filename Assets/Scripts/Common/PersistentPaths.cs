using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace UStacker.Common
{
    public static class PersistentPaths
    {
        public static string GlobalSettings => Path.Combine(DataPath, "appSettings.json");

        public static string DebugLogs => Path.Combine(DataPath, "logs.txt");
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
                    : Application.persistentDataPath;
            private set => PlayerPrefs.SetString(DATA_PATH_KEY, value);
        }

        public static async Task<bool> TrySetDataPathAsync(string newPath)
        {
            if (!FileHandling.CreateDirectoriesRecursively(newPath)) 
                return false;

            var taskList = new List<Task<bool>>
            {
                CopyFileIfExists(GlobalSettings, newPath),
                CopyFileIfExists(DebugLogs, newPath),
                CopyDirIfExists(InputPresets, newPath),
                CopyDirIfExists(GameSettingsPresets, newPath),
                CopyDirIfExists(StatCounters, newPath),
                CopyDirIfExists(Replays, newPath),
                CopyDirIfExists(RulesCustomization, newPath),
                CopyDirIfExists(StyleCustomization, newPath),
            };

            await Task.WhenAll(taskList);

            if (taskList.Any(task => !task.Result))
                return false;

            DataPath = newPath;
            return true;
        }

        private static async Task<bool> CopyFileIfExists(string filename, string destDir)
        {
            if (!File.Exists(filename)) 
                return true;

            return await FileHandling.CopyFileAsync(filename, Path.Combine(destDir, Path.GetFileName(filename)));
        }

        private static async Task<bool> CopyDirIfExists(string dirname, string destDir)
        {
            if (!Directory.Exists(dirname)) 
                return true;

            var newParent = Path.Combine(destDir, Path.GetFileName(dirname));
            try
            {
                Directory.CreateDirectory(newParent);
            }
            catch
            {
                return false;
            }

            return await FileHandling.CopyDirectoryRecursivelyAsync(dirname, newParent);
        }

    }
}