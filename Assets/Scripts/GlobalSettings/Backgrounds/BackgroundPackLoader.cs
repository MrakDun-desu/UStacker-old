using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UStacker.Common;
using UStacker.Common.Alerts;

namespace UStacker.GlobalSettings.Backgrounds
{
    public static class BackgroundPackLoader
    {
        public const string DEFAULT_PATH = "Default";
        public static readonly Dictionary<string, List<BackgroundRecord>> Backgrounds = new();

        // needs to be manually updated every time a new background is added
        private static readonly string[] SupportedBackgroundNames =
        {
            "default",
            "mainMenu",
            "gameSettings",
            "gameCustom"
        };

        public static event Action BackgroundPackChanged;

        public static IEnumerable<string> EnumerateBackgroundPacks()
        {
            return Directory.Exists(PersistentPaths.BackgroundPacks)
                ? Directory.EnumerateDirectories(PersistentPaths.BackgroundPacks).Select(Path.GetFileName)
                : Array.Empty<string>();
        }

        public static async Task Reload(string path, bool showAlert)
        {
            var defaultAlert = new Alert(
                "Switched to default background pack",
                "Backgrounds have been returned to default",
                AlertType.Info
            );
            Backgrounds.Clear();
            if (Path.GetFileName(path).Equals(DEFAULT_PATH))
            {
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                BackgroundPackChanged?.Invoke();
                if (showAlert)
                    AlertDisplayer.ShowAlert(defaultAlert);
                return;
            }

            if (!Directory.Exists(path))
            {
                BackgroundPackChanged?.Invoke();
                if (showAlert)
                    AlertDisplayer.ShowAlert(defaultAlert);
                return;
            }

            var taskList = Directory.EnumerateFiles(path)
                .Select(HandleBackgroundLoadAsync).ToList();

            taskList.AddRange(Directory.EnumerateDirectories(path).Select(HandleBackgroundFolderLoadAsync));

            await Task.WhenAll(taskList);
            if (!showAlert) return;

            BackgroundPackChanged?.Invoke();
            var shownAlert = Path.GetFileNameWithoutExtension(path) == DEFAULT_PATH
                ? defaultAlert
                : new Alert(
                    $"Background pack {Path.GetFileNameWithoutExtension(path)} loaded",
                    "Background pack has been successfully loaded and changed",
                    AlertType.Success
                );
            AlertDisplayer.ShowAlert(shownAlert);
        }

        private static async Task HandleBackgroundLoadAsync(string path)
        {
            var backgroundName = Path.GetFileNameWithoutExtension(path);
            if (!SupportedBackgroundNames.Contains(backgroundName)) return;

            var newBackground = await LoadBackgroundRecordAsync(path);
            if (newBackground is null) return;
            Backgrounds[backgroundName] = new List<BackgroundRecord>
            {
                newBackground
            };
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
            switch (FileHandling.GetFileType(path))
            {
                case FileType.Texture:
                    var newBackground = await FileHandling.LoadTextureFromUrlAsync(path);
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