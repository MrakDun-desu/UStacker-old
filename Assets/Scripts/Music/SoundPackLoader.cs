using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Blockstacker.Common;
using UnityEngine;
using UnityEngine.Networking;

namespace Blockstacker.Music
{
    [CreateAssetMenu(fileName = "SoundPackLoader", menuName = "Blockstacker/Loaders/Sound pack loader")]
    public class SoundPackLoader : ScriptableObject
    {
        public AudioClipCollection Music = new();
        public AudioClipCollection SoundEffects = new();
        public AudioClipCollection MenuSounds = new();

        public string SoundEffectsScript;

        public event Action SoundPackChanged;

        public static IEnumerable<string> EnumerateSoundPacks()
        {
            return Directory.Exists(CustomizationPaths.SoundPacks)
                ? Directory.EnumerateDirectories(CustomizationPaths.SoundPacks).Select(Path.GetFileName)
                : Array.Empty<string>();
        }

        /// <summary>
        /// Reloads music, sound effects, menu sounds and sound effects script.
        /// </summary>
        /// <param name="path">Name of the soundpack directory to load from</param>
        public async Task Reload(string path)
        {
            path = Path.Combine(CustomizationPaths.SoundPacks, path);
            if (!Directory.Exists(path)) return;
            var taskList = new List<Task>
            {
                LoadClipsFromDirectoryAsync(Path.Combine(path, CustomizationPaths.Music), Music),
                LoadClipsFromDirectoryAsync(Path.Combine(path, CustomizationPaths.SoundEffects), SoundEffects),
                LoadClipsFromDirectoryAsync(Path.Combine(path, CustomizationPaths.MenuSounds), MenuSounds),
                LoadSoundEffectsScriptAsync(Path.Combine(path, CustomizationPaths.SoundEffectScript))
            };

            await Task.WhenAll(taskList);

            SoundPackChanged?.Invoke();
        }

        private async Task LoadSoundEffectsScriptAsync(string path)
        {
            if (!File.Exists(path)) return;
            SoundEffectsScript = await File.ReadAllTextAsync(path);
        }
        
        private static async Task LoadClipsFromDirectoryAsync(string path, AudioClipCollection target)
        {
            if (!Directory.Exists(path)) return;

            var taskList = Directory.EnumerateFiles(path)
                .Select(directory => GetAudioClipAsync(directory, target));

            await Task.WhenAll(taskList);
        }


        private static async Task GetAudioClipAsync(string path, AudioClipCollection target)
        {
            var extension = Path.GetExtension(path);
            var clipName = Path.GetFileNameWithoutExtension(path);
            AudioType? audioType = extension switch
            {
                "mp3" => AudioType.MPEG,
                "ogg" => AudioType.OGGVORBIS,
                "wav" => AudioType.WAV,
                "aiff" or "aif" => AudioType.AIFF,
                "mod" => AudioType.MOD,
                "it" => AudioType.IT,
                "s3m" => AudioType.S3M,
                "xm" => AudioType.XM,
                _ => null
            };

            if (audioType == null) return;

            using var request = UnityWebRequestMultimedia.GetAudioClip(
                "file://" + path,
                (AudioType) audioType
            );

            request.SendWebRequest();

            try
            {
                while (!request.isDone) await Task.Delay(10);

                if (request.result != UnityWebRequest.Result.Success)
                    Debug.Log(request.error);
                else target.Content[clipName] = DownloadHandlerAudioClip.GetContent(request);
            }
            catch (Exception error)
            {
                Debug.Log($"{error.Message}\n{error.StackTrace}");
            }

            
        }
    }
}