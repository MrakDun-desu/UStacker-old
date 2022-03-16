using System;
using System.Collections.Generic;
using System.IO;
using Blockstacker.Settings;
using UnityEngine;

namespace Blockstacker.Loaders
{
    public static class BackgroundPackLoader
    {
        private static string _currentBackgroundPack => Path.Combine(_backgroundPackPath, AppSettings.Customization.SkinFolder);
        private static string _backgroundPackPath => Path.Combine(Application.persistentDataPath, "backgroundPacks");

        public static Dictionary<string, Texture2D> _backgrounds = new();
        public static event Action<string> BackgroundChanged;

        public static IEnumerable<string> EnumerateBackgroundPacks()
        {
            if (!Directory.Exists(_backgroundPackPath)) yield break;
            foreach (var path in Directory.EnumerateDirectories(_backgroundPackPath)) {
                var slashIndex = path.LastIndexOf("/") + 1;
                yield return path[slashIndex..];
            }
        }

        public static void Reload()
        {

        }
    }
}