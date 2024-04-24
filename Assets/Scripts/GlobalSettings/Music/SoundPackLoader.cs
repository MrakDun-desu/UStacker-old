
/************************************
SoundPackLoader.cs -- created by Marek Dančo (xdanco00)
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

namespace UStacker.GlobalSettings.Music
{
    public static class SoundPackLoader
    {
        public const string DEFAULT_PATH = "Default";
        public static readonly Dictionary<string, AudioClip> Music = new();
        public static readonly Dictionary<string, AudioClip> SoundEffects = new();

        public static string SoundEffectsScript;
        public static event Action SoundPackChanged;

        public static IEnumerable<string> EnumerateSoundPacks()
        {
            return Directory.Exists(PersistentPaths.SoundPacks)
                ? Directory.EnumerateDirectories(PersistentPaths.SoundPacks).Select(Path.GetFileName)
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
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

            if (!Directory.Exists(path))
            {
                SoundPackChanged?.Invoke();
                if (showAlert)
                    AlertDisplayer.ShowAlert(defaultAlert);
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
            AlertDisplayer.ShowAlert(shownAlert);
        }

        private static async Task LoadSoundEffectsAsync(string path)
        {
            await LoadClipsFromDirectoryAsync(path, SoundEffects);

            var scriptPath = Path.Combine(path, CustomizationFilenames.SoundEffectScript);
            if (!File.Exists(scriptPath))
                return;

            SoundEffectsScript = await File.ReadAllTextAsync(scriptPath);
        }

        private static async Task LoadMusicAsync(string path)
        {
            await LoadClipsFromDirectoryAsync(path, Music);

            MusicPlayer.Instance.Configuration.GameMusic.AddRange(Music.Keys);

            var confPath = Path.Combine(path, CustomizationFilenames.MusicConfig);
            if (!File.Exists(confPath))
                return;

            var musicConfStr = await File.ReadAllTextAsync(confPath);
            var musicConf =
                JsonConvert.DeserializeObject<MusicConfiguration>(musicConfStr,
                    StaticSettings.DefaultSerializerSettings);

            MusicPlayer.Instance.Configuration.Rewrite(musicConf);
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
            var clip = await FileHandling.LoadAudioClipFromUrlAsync(path);
            if (clip is null) return;

            target[clipName] = clip;
        }
    }
}
/************************************
end SoundPackLoader.cs
*************************************/
