using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Blockstacker.Common;
using Blockstacker.GlobalSettings.Enums;
using UnityEngine;

namespace Blockstacker.GlobalSettings.Loaders
{
    public static class SkinLoader
    {
        public static readonly Dictionary<SkinRecord, Sprite> Sprites = new();

        public static event Action SkinChanged;
        public static bool IsSkinAnimated;
        public static SkinType CurrentSkinType;

        public static IEnumerable<string> EnumerateSkins()
        {
            return Directory.Exists(CustomizationPaths.Skins)
                ? Directory.EnumerateDirectories(CustomizationPaths.Skins).Select(Path.GetFileName)
                : Array.Empty<string>();
        }

        public static async Task Reload(string path)
        {
            Sprites.Clear();
            if (!Directory.Exists(path)) return;

            await GetSkinAsync(path);

            SkinChanged?.Invoke();
        }

        private static async Task GetSkinAsync(string path)
        {
            var skinName = Path.GetFileName(path);
            var skinFileNames = Directory.EnumerateFiles(path).Select(Path.GetFileName);

            var fileNames = skinFileNames as string[] ?? skinFileNames.ToArray();
            var firstFileName = fileNames.FirstOrDefault(filename => filename.StartsWith(skinName)) ??
                                fileNames.FirstOrDefault(filename => filename.StartsWith("skin"));
            
            if (firstFileName == default)
                return;

            var firstFilePath = Path.Combine(path, firstFileName);

            if (!Path.GetExtension(firstFilePath).Equals(".png") || !Path.GetExtension(firstFilePath).Equals(".jpg"))
                return;

            var skinTexture = await GetTextureAsync(firstFilePath);

            SkinType? newSkinType;
            var isGhost = false;
            switch (skinTexture)
            {
                case {height: 128, width: 128}: 
                    isGhost = true;
                    newSkinType = SkinType.Tetrio610;
                    break;
                case {height: 256, width: 256}:
                    newSkinType = SkinType.Tetrio610;
                    break;
                case {height: 512, width: 512}:
                    isGhost = true;
                    newSkinType = SkinType.Tetrio610Connected;
                    break;
                case {height: 1024, width: 1024}:
                    newSkinType = SkinType.Tetrio610Connected;
                    break;
                case {height: 576, width: 1280}:
                    newSkinType = SkinType.JstrisConnected;
                    break;
                case {height: var x, width: var y}:
                    const int jstrisSkinRatio = 9;
                    const int tetrioSkinRatio = 12;
                    const int tetrioPixelGap = 30;
                    if (x * jstrisSkinRatio == y)
                        newSkinType = SkinType.JstrisClassic;
                    else
                    {
                        
                        var gap = x / tetrioPixelGap * tetrioSkinRatio;
                        if (x * tetrioPixelGap == y - gap)
                            newSkinType = SkinType.TetrioClassic;
                        else
                            newSkinType = null;
                    }
                    break;
                default:
                    newSkinType = null;
                    break;
            }

            switch (newSkinType)
            {
                case SkinType.Tetrio610:
                    break;
                case SkinType.Tetrio610Connected:
                    break;
                case SkinType.TetrioClassic:
                    break;
                case SkinType.JstrisClassic:
                    break;
                case SkinType.JstrisConnected:
                    break;
                case null:
                    return;
                default:
                    throw new ArgumentOutOfRangeException();
            }

        }

        private static async Task<Texture2D> GetTextureAsync(string path)
        {
            var textureData = await File.ReadAllBytesAsync(path);
            Texture2D texture = new(1, 1);
            return !texture.LoadImage(textureData) ? null : texture;
        }
    }
}