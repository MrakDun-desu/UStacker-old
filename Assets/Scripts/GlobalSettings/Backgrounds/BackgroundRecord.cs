using JetBrains.Annotations;
using UnityEngine;

namespace Blockstacker.GlobalSettings.Backgrounds
{
    public record BackgroundRecord
    {
        
        [CanBeNull] private string _videoName;
        [CanBeNull] private Texture2D _texture;
        
        public BackgroundType Type { get; }

        [CanBeNull]
        public string VideoPath
        {
            get => Type switch
            {
                BackgroundType.Video => _videoName,
                _ => null
            };
            private set => _videoName = value;
        }

        [CanBeNull]
        public Texture2D Texture
        {
            get => Type switch
            {
                BackgroundType.Texture => _texture,
                _ => null
            };
            private set => _texture = value;
        }

        public BackgroundRecord(string videoPath)
        {
            VideoPath = videoPath;
            Type = BackgroundType.Video;
        }

        public BackgroundRecord(Texture2D texture)
        {
            Texture = texture;
            Type = BackgroundType.Texture;
        }

        public enum BackgroundType : byte
        {
            Video,
            Texture
        }
    }
}