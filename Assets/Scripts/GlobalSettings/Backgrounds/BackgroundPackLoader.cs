using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Blockstacker.Common;

namespace Blockstacker.GlobalSettings.Backgrounds
{
    public static class BackgroundPackLoader
    {
        public static readonly Dictionary<string, List<BackgroundRecord>> Backgrounds = new();
        
        public static event Action BackgroundPackChanged;

        // needs to be manually updated every time a new background is added
        private static readonly string[] SupportedBackgroundNames =
        {
            "default",
            "mainMenu",
            "globalSettings",
            "gameSettings",
            "gameCustom"
        };
        
        public static IEnumerable<string> EnumerateBackgroundPacks()
        {
            return Directory.Exists(CustomizationPaths.BackgroundPacks)
                ? Directory.EnumerateDirectories(CustomizationPaths.BackgroundPacks).Select(Path.GetFileName)
                : Array.Empty<string>();
        }

        public static async Task Reload(string path)
        {
            Backgrounds.Clear();
            if (!Directory.Exists(path)) return;

            var taskList = Directory.EnumerateFiles(path)
                .Select(HandleBackgroundLoadAsync).ToList();

            taskList.AddRange(Directory.EnumerateDirectories(path).Select(HandleBackgroundFolderLoadAsync));

            await Task.WhenAll(taskList);
            BackgroundPackChanged?.Invoke();
        }

        private static async Task HandleBackgroundLoadAsync(string path)
        {
            var backgroundName = Path.GetFileNameWithoutExtension(path);
            if (!SupportedBackgroundNames.Contains(backgroundName)) return;

            var newBackground = await LoadBackgroundRecordAsync(path);
            if (newBackground is null) return; 
            Backgrounds[backgroundName] = new List<BackgroundRecord> {newBackground};
        }

        private static async Task HandleBackgroundFolderLoadAsync(string path)
        {
            var backgroundName = Path.GetFileNameWithoutExtension(path);
            if (!SupportedBackgroundNames.Contains(backgroundName)) return;

            Backgrounds[backgroundName] = new List<BackgroundRecord>();
            foreach (var file in Directory.EnumerateFiles(path))
            {
                var newBackground = await LoadBackgroundRecordAsync(file);
                if (newBackground is null) continue;
                Backgrounds[backgroundName].Add(newBackground);
            }

            if (Backgrounds[backgroundName].Count == 0)
                Backgrounds.Remove(backgroundName);
        }

        private static async Task<BackgroundRecord> LoadBackgroundRecordAsync(string path)
        {
            var type = FileLoading.GetFileType(path);
            switch (type)
            {
                case FileType.Texture:
                    var newBackground = await FileLoading.LoadTextureFromUrl(path);
                    return new BackgroundRecord(newBackground);
                case FileType.Video:
                    return new BackgroundRecord(path);
                case FileType.AudioClip:
                case FileType.Invalid:
                    return null;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}