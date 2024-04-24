
/************************************
BackgroundRecord.cs -- created by Marek Dančo (xdanco00)
*************************************/
using System;
using JetBrains.Annotations;
using UnityEngine;

namespace UStacker.GlobalSettings.Backgrounds
{
    [Serializable]
    public record BackgroundRecord
    {
        [CanBeNull] [SerializeField] private string _videoPath;
        [CanBeNull] [SerializeField] private Texture2D _texture;
        [SerializeField] private BackgroundType _type;

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

        public BackgroundType Type
        {
            get => _type;
            private set => _type = value;
        }

        [CanBeNull]
        public string VideoPath
        {
            get => Type switch
            {
                BackgroundType.Video => _videoPath,
                _ => null
            };
            private set => _videoPath = value;
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
    }

    public enum BackgroundType : byte
    {
        Texture,
        Video
    }
}
/************************************
end BackgroundRecord.cs
*************************************/
