using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace Blockstacker.Loaders
{
    public static class BackgroundPackLoader
    {
        public static readonly Dictionary<string, Texture2D> BackgroundImages = new();
        public static readonly Dictionary<string, string> BackgroundVideos = new();

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

        public static async Task Reload(string path)
        {
            BackgroundImages.Clear();
            BackgroundVideos.Clear();
            if (!Directory.Exists(path)) return;
            
            var taskList = Directory.EnumerateFiles(path)
                .Select(HandleBackgroundLoadAsync);

            await Task.WhenAll(taskList);
            BackgroundPackChanged?.Invoke();
        }

        private static async Task HandleBackgroundLoadAsync(string path)
        {
            var extension = Path.GetExtension(path).ToLower().Remove(0, 1);
            var backgroundName = Path.GetFileNameWithoutExtension(path);

            var type = extension switch
            {
                "jpg" => BackgroundType.Image,
                "png" => BackgroundType.Image,
                "avi" => BackgroundType.Video,
                "dv" => BackgroundType.Video,
                "m4v" => BackgroundType.Video,
                "mov" => BackgroundType.Video,
                "mp4" => BackgroundType.Video, // definitely works
                "mpg" => BackgroundType.Video,
                "mpeg" => BackgroundType.Video,
                "ogv" => BackgroundType.Video,
                "vp8" => BackgroundType.Video,
                "wmv" => BackgroundType.Video,
                _ => BackgroundType.Invalid
            };

            switch (type)
            {
                case BackgroundType.Image:
                    var textureData = await File.ReadAllBytesAsync(path);
                    Texture2D texture = new(1, 1);
                    var newBackground = texture.LoadImage(textureData, false) ? texture : null;
                    BackgroundImages[backgroundName] = newBackground;
                    break;
                case BackgroundType.Video:
                    BackgroundVideos[backgroundName] = path;
                    break;
                case BackgroundType.Invalid:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private enum BackgroundType
        {
            Image,
            Video,
            Invalid
        }

    }
}