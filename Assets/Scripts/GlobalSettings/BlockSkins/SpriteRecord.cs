using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blockstacker.Common;
using Newtonsoft.Json;
using UnityEngine;

namespace Blockstacker.GlobalSettings.BlockSkins
{
    [Serializable]
    public class SpriteRecord
    {
        public string Filename;
        public bool LoadFromUrl;
        public float PixelsPerUnit = 64;
        public Vector2 PivotPoint = new(.5f, .5f);
        public Vector2 SpriteStart = new(0, 0);
        public Vector2 SpriteSize = new(64, 64);

        [JsonIgnore]
        public Sprite Sprite;

        public bool TryLoadSpriteFromDict(Dictionary<string, Texture2D> existingTextures)
        {
            if (!existingTextures.TryGetValue(Filename, out var sourceTexture))
                return false;

            var spriteRect = new Rect(SpriteStart, SpriteSize);
            Sprite = Sprite.Create(sourceTexture, spriteRect, PivotPoint, PixelsPerUnit, 0, SpriteMeshType.FullRect);
            return Sprite != null;
        }
        
        public async Task<bool> TryLoadSpriteAsync()
        {
            var sourceTexture = await FileLoading.LoadTextureFromUrl(Filename, !LoadFromUrl);
                if (sourceTexture is null)
                    return false;

            var spriteRect = new Rect(SpriteStart, SpriteSize);
            Sprite = Sprite.Create(sourceTexture, spriteRect, PivotPoint, PixelsPerUnit, 0, SpriteMeshType.FullRect);
            return true;
        }
    }
}