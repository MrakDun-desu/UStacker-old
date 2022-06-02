using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace Blockstacker.Loaders
{
    public static class BackgroundPackLoader
    {
        public static Dictionary<string, Texture2D> Backgrounds = new();

        private static string BackgroundPackPath => Path.Combine(Application.persistentDataPath, "backgroundPacks");
        public static event Action BackgroundPackChanged;

        public static IEnumerable<string> EnumerateBackgroundPacks()
        {
            if (!Directory.Exists(BackgroundPackPath)) yield break;
            foreach (var path in Directory.EnumerateDirectories(BackgroundPackPath))
            {
                var slashIndex = path.LastIndexOfAny(new[] {'/', '\\'}) + 1;
                yield return path[slashIndex..];
            }
        }

        public static void Reload(string backgroundPath)
        {
            Backgrounds.Clear();
            _ = GetBackgroundsRecursivelyAsync(1, backgroundPath);
        }

        private static async Task GetBackgroundsRecursivelyAsync(int recursionLevel, string rootPath, string path = "")
        {
            if (recursionLevel-- <= 0) return;
            List<Task> taskList = new();

            foreach (var dir in Directory.EnumerateDirectories(Path.Combine(rootPath, path)))
            {
                var slashIndex = dir.LastIndexOfAny(new[] {'\\', '/'}) + 1;
                taskList.Add(GetBackgroundsRecursivelyAsync(recursionLevel, path + '/' + dir[slashIndex..]));
            }

            foreach (var filePath in Directory.EnumerateFiles(Path.Combine(rootPath, path)))
            {
                taskList.Add(HandleBackgroundLoadAsync(filePath));
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
            var textureData = await File.ReadAllBytesAsync(path);
            Texture2D texture = new(1, 1);
            return texture.LoadImage(textureData, false) ? texture : null;
        }
    }
}