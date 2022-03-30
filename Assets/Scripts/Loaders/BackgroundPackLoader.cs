using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Blockstacker.GlobalSettings;
using UnityEngine;

namespace Blockstacker.Loaders
{
    public static class BackgroundPackLoader
    {
        private static string CurrentBackgroundPack => Path.Combine(BackgroundPackPath, AppSettings.Customization.BackgroundFolder);
        private static string BackgroundPackPath => Path.Combine(Application.persistentDataPath, "backgroundPacks");

        public static Dictionary<string, Texture2D> Backgrounds = new();
        public static event Action BackgroundPackChanged;

        public static IEnumerable<string> EnumerateBackgroundPacks()
        {
            if (!Directory.Exists(BackgroundPackPath)) yield break;
            foreach (var path in Directory.EnumerateDirectories(BackgroundPackPath)) {
                var slashIndex = path.LastIndexOfAny(new char[] { '/', '\\' }) + 1;
                yield return path[slashIndex..];
            }
        }

        public static void Reload()
        {
            Backgrounds.Clear();
            _ = GetBackgroundsRecursivelyAsync(1);
        }

        private static async Task GetBackgroundsRecursivelyAsync(int recursionLevel, string path = "")
        {
            if (recursionLevel-- <= 0) return;
            List<Task> taskList = new();

            foreach (var dir in Directory.EnumerateDirectories(Path.Combine(CurrentBackgroundPack, path))) {
                var slashIndex = dir.LastIndexOfAny(new char[] { '\\', '/' }) + 1;
                taskList.Add(GetBackgroundsRecursivelyAsync(recursionLevel, path + '/' + dir[slashIndex..]));
            }

            foreach (var filePath in Directory.EnumerateFiles(Path.Combine(CurrentBackgroundPack, path))) {
                var slashIndex = filePath.LastIndexOfAny(new char[] { '\\', '/' }) + 1;
                taskList.Add(HandleBackgroundLoadAsync(filePath[slashIndex..]));
            }

            await Task.WhenAll(taskList);
            BackgroundPackChanged?.Invoke();
        }

        private static async Task HandleBackgroundLoadAsync(string path)
        {
            var newBackground = await GetBackgroundAsync(path);
            var dotIndex = path.LastIndexOf('.');
            Backgrounds.TryAdd(path[..dotIndex], newBackground);
        }

        private static async Task<Texture2D> GetBackgroundAsync(string path)
        {
            var textureData = await File.ReadAllBytesAsync(Path.Combine(CurrentBackgroundPack, path));
            Texture2D texture = new(1, 1);
            if (!texture.LoadImage(textureData, false)) {
                return null;
            }
            return texture;
        }


    }
}