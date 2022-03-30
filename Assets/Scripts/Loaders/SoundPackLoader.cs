using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Blockstacker.GlobalSettings;
using UnityEngine;
using UnityEngine.Networking;

namespace Blockstacker.Loaders
{
    public static class SoundPackLoader
    {
        private static string CurrentSoundPack => Path.Combine(SoundPackPath, AppSettings.Customization.SoundPackFolder);
        private static string SoundPackPath => Path.Combine(Application.persistentDataPath, "soundPacks");

        public static Dictionary<string, AudioClip> Sounds = new();
        public static event Action SoundPackChanged;

        public static IEnumerable<string> EnumerateSoundPacks()
        {
            if (!Directory.Exists(SoundPackPath)) yield break;
            foreach (var path in Directory.EnumerateDirectories(SoundPackPath)) {
                var slashIndex = path.LastIndexOfAny(new char[] { '/', '\\' }) + 1;
                yield return path[slashIndex..];
            }
        }

        public static void Reload()
        {
            Sounds.Clear();
            _ = GetClipsRecursivelyAsync(1);
        }

        private static async Task GetClipsRecursivelyAsync(int recursionLevel, string path = "")
        {
            if (recursionLevel-- <= 0) return;
            List<Task> taskList = new();
            foreach (var dir in Directory.EnumerateDirectories(Path.Combine(CurrentSoundPack, path))) {
                var slashIndex = dir.LastIndexOfAny(new char[] { '\\', '/' }) + 1;
                taskList.Add(GetClipsRecursivelyAsync(recursionLevel, path + '/' + dir[slashIndex..]));
            }

            foreach (var filePath in Directory.EnumerateFiles(Path.Combine(CurrentSoundPack, path))) {
                var slashIndex = filePath.LastIndexOfAny(new char[] { '\\', '/' }) + 1;
                taskList.Add(HandleAudioClipLoadAsync(filePath[slashIndex..]));
            }

            await Task.WhenAll(taskList);
            SoundPackChanged?.Invoke();
        }

        private static async Task HandleAudioClipLoadAsync(string path)
        {
            var newClip = await GetAudioClipAsync(path);
            Sounds.TryAdd(path, newClip);
        }

        private static async Task<AudioClip> GetAudioClipAsync(string path)
        {
            int dotIndex = path.LastIndexOf('.') + 1;
            var extension = path[dotIndex..];
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

            if (audioType == null) return null;

            using UnityWebRequest request = UnityWebRequestMultimedia.GetAudioClip(
                "file://" + Path.Combine(CurrentSoundPack, path),
                (AudioType)audioType
                );

            request.SendWebRequest();

            AudioClip clip = null;

            try {
                while (!request.isDone) await Task.Delay(10);

                if (request.result != UnityWebRequest.Result.Success) {
                    Debug.Log(request.error);
                }
                else clip = DownloadHandlerAudioClip.GetContent(request);
            }
            catch (Exception error) {
                Debug.Log($"{error.Message}\n{error.StackTrace}");
            }

            return clip;
        }
    }
}