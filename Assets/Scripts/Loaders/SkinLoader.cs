using System;
using System.Collections.Generic;
using System.IO;
using Blockstacker.Settings;
using UnityEngine;

namespace Blockstacker.Loaders
{
    public static class SkinLoader
    {
        private static string _currentSkin => Path.Combine(_skinPath, AppSettings.Customization.SkinFolder);
        private static string _skinPath => Path.Combine(Application.persistentDataPath, "skins");

        public static Dictionary<string, Sprite> Skins = new();
        public static event Action<string> SkinChanged;

        public static IEnumerable<string> EnumerateSkins()
        {
            if (!Directory.Exists(_skinPath)) yield break;
            foreach (var path in Directory.EnumerateDirectories(_skinPath)) {
                var slashIndex = path.LastIndexOfAny(new char[] { '/', '\\' }) + 1;
                yield return path[slashIndex..];
            }
        }

        public static void Reload()
        {

        }
    }
}