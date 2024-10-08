
/************************************
SkinLoader.cs -- created by Marek Dančo (xdanco00)
*************************************/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UStacker.Common;
using UStacker.Common.Alerts;

namespace UStacker.GlobalSettings.BlockSkins
{
    public static class SkinLoader
    {
        public const string DEFAULT_PATH = "Default";
        public static List<SkinRecord> SkinRecords { get; private set; } = new();

        public static event Action SkinChanged;

        public static IEnumerable<string> EnumerateSkins()
        {
            return Directory.Exists(PersistentPaths.Skins)
                ? Directory.EnumerateDirectories(PersistentPaths.Skins).Select(Path.GetFileName)
                : Array.Empty<string>();
        }

        public static async Task ReloadAsync(string path, bool showAlert)
        {
            var defaultAlert = new Alert(
                "Switched to default block skin",
                "Block skin has been returned to default",
                AlertType.Info
            );
            SkinRecords.Clear();
            if (Path.GetFileName(path).Equals(DEFAULT_PATH))
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

            if (!Directory.Exists(path))
            {
                SkinChanged?.Invoke();
                if (showAlert)
                    AlertDisplayer.ShowAlert(defaultAlert);
                return;
            }

            await GetSkinAsync(path);

            SkinChanged?.Invoke();
            if (!showAlert) return;

            var shownAlert = Path.GetFileNameWithoutExtension(path) == DEFAULT_PATH
                ? defaultAlert
                : new Alert(
                    $"Block skin {Path.GetFileNameWithoutExtension(path)} loaded",
                    "Skin has been successfully loaded and changed",
                    AlertType.Success
                );
            AlertDisplayer.ShowAlert(shownAlert);
        }

        private static async Task GetSkinAsync(string path)
        {
            var configFilePath = Path.Combine(path, CustomizationFilenames.SkinConfiguration);
            if (!File.Exists(configFilePath)) return;

            var skinJson = await File.ReadAllTextAsync(configFilePath);
            SkinRecords =
                JsonConvert.DeserializeObject<List<SkinRecord>>(skinJson, StaticSettings.DefaultSerializerSettings);
            SkinRecords ??= new List<SkinRecord>();

            var existingTextures = await LoadAllTexturesAsync(SkinRecords, path);
            foreach (var skinRecord in SkinRecords)
                LoadSkinByRecord(skinRecord, existingTextures);
        }

        private static void LoadSkinByRecord(SkinRecord skinRecord, Dictionary<string, Texture2D> existingTextures)
        {
            if (skinRecord.IsConnected)
            {
                var availableKeys = skinRecord.ConnectedSprites.Keys;
                foreach (var key in availableKeys)
                {
                    var spriteRecords = skinRecord.ConnectedSprites[key];
                    for (var i = 0; i < spriteRecords.Count; i++)
                    {
                        if (spriteRecords[i].TryLoadSpriteFromDict(existingTextures)) continue;

                        // if we couldn't load the sprite record, we remove it from the collection
                        spriteRecords.RemoveAt(i);
                        i--;
                    }

                    // if there were no valid sprites, we remove the whole collection
                    if (spriteRecords.Count == 0)
                        skinRecord.ConnectedSprites.Remove(key);
                }

                if (skinRecord.ConnectedSprites.Count == 0)
                    SkinRecords.Remove(skinRecord);
            }
            else
            {
                foreach (var spriteRecord in skinRecord.Sprites)
                    if (!spriteRecord.TryLoadSpriteFromDict(existingTextures))
                        skinRecord.Sprites.Remove(spriteRecord);

                if (skinRecord.Sprites.Count == 0)
                    SkinRecords.Remove(skinRecord);
            }
        }

        private static async Task<Dictionary<string, Texture2D>> LoadAllTexturesAsync(
            ICollection<SkinRecord> skinRecords, string path)
        {
            var taskList = new List<Task>();
            var output = new Dictionary<string, Texture2D>();
            var fileList = new List<string>();
            foreach (var skinRecord in skinRecords)
                if (skinRecord.IsConnected)
                {
                    var spriteRecords = skinRecords
                        .SelectMany(sr => sr.ConnectedSprites.Values.SelectMany(sprite => sprite));

                    foreach (var spriteRecord in spriteRecords)
                    {
                        if (fileList.Contains(spriteRecord.Filename)) continue;

                        fileList.Add(spriteRecord.Filename);
                        taskList.Add(LoadTextureToDictionary(spriteRecord.Filename, !spriteRecord.LoadFromUrl, output,
                            path));
                    }
                }
                else
                {
                    var spriteRecords = skinRecords
                        .SelectMany(sr => sr.Sprites);

                    foreach (var spriteRecord in spriteRecords)
                    {
                        if (fileList.Contains(spriteRecord.Filename)) continue;

                        fileList.Add(spriteRecord.Filename);
                        taskList.Add(LoadTextureToDictionary(spriteRecord.Filename, !spriteRecord.LoadFromUrl, output,
                            path));
                    }
                }

            await Task.WhenAll(taskList);
            return output;
        }

        private static async Task LoadTextureToDictionary(string filename, bool isFile,
            IDictionary<string, Texture2D> textures, string path)
        {
            var actualFilename = isFile ? Path.Combine(path, filename) : filename;

            var texture = await FileHandling.LoadTextureFromUrlAsync(actualFilename, isFile);

            if (texture is not null)
                textures[filename] = texture;
        }
    }
}
/************************************
end SkinLoader.cs
*************************************/
