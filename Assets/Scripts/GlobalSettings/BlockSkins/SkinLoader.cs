using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Blockstacker.Common;
using Unity.Plastic.Newtonsoft.Json;
using UnityEngine;

namespace Blockstacker.GlobalSettings.BlockSkins
{
    public static class SkinLoader
    {
        public static List<SkinRecord> SkinRecords { get; private set; } = new();

        public static event Action SkinChanged;

        public static IEnumerable<string> EnumerateSkins()
        {
            return Directory.Exists(CustomizationPaths.Skins)
                ? Directory.EnumerateDirectories(CustomizationPaths.Skins).Select(Path.GetFileName)
                : Array.Empty<string>();
        }

        public static async Task Reload(string path)
        {
            SkinRecords.Clear();
            if (!Directory.Exists(path)) return;

            await GetSkinAsync(path);

            SkinChanged?.Invoke();
        }

        private static async Task GetSkinAsync(string path)
        {
            var configFilePath = Path.Combine(path, CustomizationPaths.SkinConfiguration);
            if (!File.Exists(configFilePath)) return;

            var skinJson = await File.ReadAllTextAsync(configFilePath);
            SkinRecords = JsonConvert.DeserializeObject<List<SkinRecord>>(skinJson, StaticSettings.JsonSerializerSettings);
            SkinRecords ??= new List<SkinRecord>();

            var existingTextures = new Dictionary<string, Texture2D>();
            var skinsLoaded = SkinRecords.Select(skinRecord => LoadSkinByRecord(skinRecord, existingTextures));
            
            await Task.WhenAll(skinsLoaded);
        }

        private static async Task LoadSkinByRecord(SkinRecord skinRecord, Dictionary<string, Texture2D> existingTextures)
        {
            if (skinRecord.IsConnected)
            {
                foreach (var connectedSprite in skinRecord.ConnectedSprites)
                {
                    foreach (var spriteRecord in connectedSprite.Sprites)
                    {
                        if (!await spriteRecord.TryLoadSpriteAsync(existingTextures))
                            connectedSprite.Sprites.Remove(spriteRecord);
                    }
                    if (connectedSprite.Sprites.Count == 0)
                        skinRecord.ConnectedSprites.Remove(connectedSprite);
                }
                if (skinRecord.ConnectedSprites.Count == 0)
                    SkinRecords.Remove(skinRecord);
            }
            else
            {
                foreach (var spriteRecord in skinRecord.Sprites)
                {
                    if (!await spriteRecord.TryLoadSpriteAsync(existingTextures))
                        skinRecord.Sprites.Remove(spriteRecord);
                }
                if (skinRecord.Sprites.Count == 0)
                    SkinRecords.Remove(skinRecord);
            }
        }

    }
}