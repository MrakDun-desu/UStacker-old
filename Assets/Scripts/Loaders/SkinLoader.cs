using System.Collections.Generic;
using System.IO;
using Blockstacker.Settings;
using UnityEngine;

namespace Blockstacker.Loaders
{
    public static class SkinLoader
    {
        private static Dictionary<string, Sprite> _skins = new();
        private static string _currentSkin => Path.Combine(_skinPath, AppSettings.Customization.SkinFolder);
        private static string _skinPath => Path.Combine(Application.persistentDataPath, "skins");

        public static IEnumerable<string> EnumerateSkins()
        {
            if (!Directory.Exists(_skinPath)) yield break;
            foreach (var path in Directory.EnumerateDirectories(_skinPath)) {
                var slashIndex = path.LastIndexOf("/") + 1;
                yield return path[slashIndex..];
            }
        }
    }
}