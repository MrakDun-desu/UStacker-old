using System;
using System.Collections.Generic;
using System.IO;
using Blockstacker.Settings;
using UnityEngine;

namespace Blockstacker.Loaders
{
    public static class SoundPackLoader
    {
        private static string _currentSoundPack => Path.Combine(_soundPackPath, AppSettings.Customization.SkinFolder);
        private static string _soundPackPath => Path.Combine(Application.persistentDataPath, "soundPacks");

        public static Dictionary<string, AudioClip> _sounds = new();
        public static event Action<string> SoundChanged;

        public static IEnumerable<string> EnumerateSoundPacks()
        {
            if (!Directory.Exists(_soundPackPath)) yield break;
            foreach (var path in Directory.EnumerateDirectories(_soundPackPath)) {
                var slashIndex = path.LastIndexOf("/") + 1;
                yield return path[slashIndex..];
            }
        }

        public static void Reload()
        {

        }
    }
}