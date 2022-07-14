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
        public static readonly List<SkinRecord> SkinRecords = new();

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
            SkinRecords.Add(
                new SkinRecord
                {
                    File = "file.jpg",
                    PieceType = "IPiece",
                    ConnectedSprites = Array.Empty<ConnectedSprite>(),
                    SpriteRecord = new SpriteRecord
                    {
                        PixelsPerUnit = 64, 
                        PivotPoint = new Vector2Int(32, 32), 
                        SpriteSize = new Vector2Int(64, 64), 
                        SpriteStart = new Vector2Int(0, 0),
                    }
                });

            await File.WriteAllTextAsync(Path.Combine(path, "output.json"), JsonConvert.SerializeObject(SkinRecords, StaticSettings.JsonSerializerSettings));
        }

    }
}