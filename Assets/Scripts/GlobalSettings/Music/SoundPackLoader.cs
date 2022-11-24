using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Blockstacker.Common;
using Blockstacker.Common.Alerts;
using Blockstacker.Common.Extensions;
using Newtonsoft.Json;
using NLua;
using NLua.Exceptions;
using UnityEngine;

namespace Blockstacker.GlobalSettings.Music
{
    public static class SoundPackLoader
    {
        public static readonly Dictionary<string, AudioClip> Music = new();
        public static readonly Dictionary<string, AudioClip> SoundEffects = new();

        public static string SoundEffectsScript;
        public static event Action SoundPackChanged;
        public const string DEFAULT_PATH = "Default";

        public static IEnumerable<string> EnumerateSoundPacks()
        {
            return Directory.Exists(CustomizationPaths.SoundPacks)
                ? Directory.EnumerateDirectories(CustomizationPaths.SoundPacks).Select(Path.GetFileName)
                : Array.Empty<string>();
        }

        public static async Task Reload(string path, bool showAlert)
        {
            var defaultAlert = new Alert(
                "Switched to default sound pack",
                "Sound pack has been returned to default",
                AlertType.Info
            );
            Music.Clear();
            SoundEffects.Clear();
            SoundEffectsScript = "";
            if (Path.GetFileName(path).Equals(DEFAULT_PATH))
            {
                if (!Directory.Exists(path))
                {
                    SoundPackChanged?.Invoke();
                    if (showAlert)
                        _ = AlertDisplayer.Instance.ShowAlert(defaultAlert);
                    return;
                }
            }

            if (!Directory.Exists(path))
            {
                SoundPackChanged?.Invoke();
                if (showAlert)
                    _ = AlertDisplayer.Instance.ShowAlert(defaultAlert);
                return;
            }
            var taskList = new List<Task>
            {
                LoadSoundEffectsAsync(Path.Combine(path, CustomizationFilenames.SoundEffects)),
                LoadMusicAsync(Path.Combine(path, CustomizationFilenames.Music))
            };

            await Task.WhenAll(taskList);

            SoundPackChanged?.Invoke();
            if (!showAlert) return;
            
            var shownAlert = Path.GetFileNameWithoutExtension(path) == DEFAULT_PATH
                ? defaultAlert
                : new Alert(
                    $"Sound pack {Path.GetFileNameWithoutExtension(path)} loaded",
                    "Sound pack has been successfully loaded and changed",
                    AlertType.Success
                );
            _ = AlertDisplayer.Instance.ShowAlert(shownAlert);
        }

        private static async Task LoadSoundEffectsAsync(string path)
        {
            await LoadClipsFromDirectoryAsync(path, SoundEffects);

            var scriptPath = Path.Combine(path, CustomizationFilenames.SoundEffectScript);
            if (!File.Exists(scriptPath))
                return;

            SoundEffectsScript = await File.ReadAllTextAsync(scriptPath);
            var lua = new Lua();
            lua.RestrictMaliciousFunctions();
            try
            {
                lua.DoString(SoundEffectsScript);
            }
            catch (LuaException ex)
            {
                _ = AlertDisplayer.Instance.ShowAlert(new Alert(
                    "Error loading script!",
                    $"Error loading sound effects script.\nLua error: {ex.Message}",
                    AlertType.Error));
                SoundEffectsScript = null;
            }
        }

        private static async Task LoadMusicAsync(string path)
        {
            await LoadClipsFromDirectoryAsync(path, Music);

            MusicPlayer.Configuration.GameMusic.AddRange(Music.Keys);

            var confPath = Path.Combine(path, CustomizationFilenames.MusicConfig);
            if (!File.Exists(confPath))
                return;

            var musicConfStr = await File.ReadAllTextAsync(confPath);
            var musicConf = JsonConvert.DeserializeObject<MusicConfiguration>(musicConfStr, StaticSettings.JsonSerializerSettings);

            MusicPlayer.Configuration.Rewrite(musicConf);
        }

        private static async Task LoadClipsFromDirectoryAsync(string path, IDictionary<string, AudioClip> target)
        {
            if (!Directory.Exists(path)) return;

            var taskList = Directory.EnumerateFiles(path)
                .Select(directory => GetAudioClipAsync(directory, target));

            await Task.WhenAll(taskList);
        }

        private static async Task GetAudioClipAsync(string path, IDictionary<string, AudioClip> target)
        {
            var clipName = Path.GetFileNameWithoutExtension(path);
            var clip = await FileLoading.LoadAudioClipFromUrl(path);
            if (clip is null) return;

            target[clipName] = clip;
        }
    }
}