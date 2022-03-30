using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Blockstacker.GlobalSettings;
using UnityEngine;

namespace Blockstacker.Loaders
{
    public static class SkinLoader
    {
        // TODO
        // private static readonly string _blockSkinName = "blockSkin";
        // private static readonly string[] _blockSpriteNames = new string[] {
        // };

        // private static readonly float _defaultPixelsPerUnit = 64;
        private static string CurrentSkin => Path.Combine(SkinPath, AppSettings.Customization.SkinFolder);
        private static string SkinPath => Path.Combine(Application.persistentDataPath, "skins");

        public static Dictionary<string, Sprite> Sprites = new();
        public static event Action SkinChanged;

        public static IEnumerable<string> EnumerateSkins()
        {
            if (!Directory.Exists(SkinPath)) yield break;
            foreach (var path in Directory.EnumerateDirectories(SkinPath)) {
                var slashIndex = path.LastIndexOfAny(new char[] { '/', '\\' }) + 1;
                yield return path[slashIndex..];
            }
        }

        public static void Reload()
        {
            Sprites.Clear();
            _ = GetSkinsRecursivelyAsync(1);
        }

        private static async Task GetSkinsRecursivelyAsync(int recursionLevel, string path = "")
        {
            if (recursionLevel-- <= 0) return;
            List<Task> taskList = new();

            foreach (var dir in Directory.EnumerateDirectories(Path.Combine(CurrentSkin, path))) {
                var slashIndex = dir.LastIndexOfAny(new char[] { '\\', '/' }) + 1;
                taskList.Add(GetSkinsRecursivelyAsync(recursionLevel, path + '/' + dir[slashIndex..]));
            }

            foreach (var filePath in Directory.EnumerateFiles(Path.Combine(CurrentSkin, path))) {
                var slashIndex = filePath.LastIndexOfAny(new char[] { '\\', '/' }) + 1;
                taskList.Add(HandleLoadSpriteAsync(filePath[slashIndex..]));
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
            var textureData = await File.ReadAllBytesAsync(Path.Combine(CurrentSkin, path));
            Texture2D texture = new(1, 1);
            if (!texture.LoadImage(textureData, false)) {
                return null;
            }
            return texture;
        }
    }
}