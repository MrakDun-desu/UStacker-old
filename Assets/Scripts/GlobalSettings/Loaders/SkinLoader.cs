using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Blockstacker.Common;
using UnityEngine;

namespace Blockstacker.GlobalSettings.Loaders
{
    public static class SkinLoader
    {
        public static readonly Dictionary<string, Sprite> Sprites = new();
        // TODO add skin loading and names
        // private static readonly string _blockSkinName = "blockSkin";
        // private static readonly string[] _blockSpriteNames = new string[] {
        // };

        // private static readonly float _defaultPixelsPerUnit = 64;
        public static event Action SkinChanged;

        public static IEnumerable<string> EnumerateSkins()
        {
            if (!Directory.Exists(CustomizationPaths.Skins)) yield break;
            foreach (var path in Directory.EnumerateDirectories(CustomizationPaths.Skins))
            {
                var slashIndex = path.LastIndexOfAny(new[] {'/', '\\'}) + 1;
                yield return path[slashIndex..];
            }
        }

        public static void Reload(string path)
        {
            Sprites.Clear();
            _ = GetSkinsRecursivelyAsync(1, path);
        }

        private static async Task GetSkinsRecursivelyAsync(int recursionLevel, string rootPath, string path = "")
        {
            if (recursionLevel-- <= 0) return;
            List<Task> taskList = new();

            foreach (var dir in Directory.EnumerateDirectories(Path.Combine(rootPath, path)))
            {
                var slashIndex = dir.LastIndexOfAny(new[] {'\\', '/'}) + 1;
                taskList.Add(GetSkinsRecursivelyAsync(recursionLevel, path + '/' + dir[slashIndex..]));
            }

            foreach (var filePath in Directory.EnumerateFiles(Path.Combine(rootPath, path)))
            {
                taskList.Add(HandleLoadSpriteAsync(filePath));
            }

            await Task.WhenAll(taskList);
            SkinChanged?.Invoke();
        }

        private static async Task HandleLoadSpriteAsync(string path)
        {
            var newTexture = await GetTextureAsync(path);
            var dotIndex = path.LastIndexOf('.');
            var spriteName = path[..dotIndex];
            // TODO
            // if (spriteName.Equals(_blockSkinName)) {
            // }
        }

        private static void LoadBlockSkinFromTexture(Texture2D texture)
        {
            // TODO
        }

        private static async Task<Texture2D> GetTextureAsync(string path)
        {
            var textureData = await File.ReadAllBytesAsync(path);
            Texture2D texture = new(1, 1);
            if (!texture.LoadImage(textureData, false)) return null;
            return texture;
        }
    }
}