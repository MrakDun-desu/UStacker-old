using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blockstacker.Common;
using Unity.Plastic.Newtonsoft.Json;
using UnityEngine;

namespace Blockstacker.GlobalSettings.BlockSkins
{
    [Serializable]
    public record SpriteRecord
    {
        public string Filename;
        public bool LoadFromUrl;
        public float PixelsPerUnit = 64;
        public Vector2 PivotPoint = new(32, 32);
        public Vector2 SpriteStart = new(0, 0);
        public Vector2 SpriteSize = new(64, 64);

        [JsonIgnore]
        public Sprite Sprite;

        public async Task<bool> TryLoadSpriteAsync(Dictionary<string, Texture2D> existingTextures)
        {
            Texture2D sourceTexture;
            if (existingTextures.ContainsKey(Filename))
                sourceTexture = existingTextures[Filename];
            else
            {
                sourceTexture = await FileLoading.LoadTextureFromUrl(Filename, !LoadFromUrl);
                if (sourceTexture == null) return false;
                existingTextures.Add(Filename, sourceTexture);
            }
            
            var spriteRect = new Rect(SpriteStart, SpriteSize);
            Sprite = Sprite.Create(sourceTexture, spriteRect, PivotPoint, PixelsPerUnit);
            return true;
        }
    }
}