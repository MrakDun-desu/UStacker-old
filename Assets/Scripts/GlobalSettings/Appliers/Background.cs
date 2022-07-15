using System;
using Blockstacker.Common.Extensions;
using Blockstacker.GlobalSettings.Backgrounds;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using Random = UnityEngine.Random;

namespace Blockstacker.GlobalSettings.Appliers
{
    [RequireComponent(typeof(RawImage), typeof(VideoPlayer))]
    public class Background : MonoBehaviour
    {
        [SerializeField] private string _backgroundName;

        private RawImage _backgroundImage;
        private VideoPlayer _videoPlayer;
        private Texture _defaultTexture;

        private void Awake()
        {
            _backgroundImage = GetComponent<RawImage>();
            _videoPlayer = GetComponent<VideoPlayer>();
            _defaultTexture = _backgroundImage.texture;
        }

        private void OnEnable()
        {
            OnVisibilityChanged(AppSettings.Video.BackgroundVisibility);
            OnBackgroundChanged();
            BackgroundVisibilityApplier.VisibilityChanged += OnVisibilityChanged;
            BackgroundPackLoader.BackgroundPackChanged += OnBackgroundChanged;
        }

        private void OnDisable()
        {
            BackgroundVisibilityApplier.VisibilityChanged -= OnVisibilityChanged;
            BackgroundPackLoader.BackgroundPackChanged -= OnBackgroundChanged;
        }

        private void OnVisibilityChanged(float newValue)
        {
            _backgroundImage.color = _backgroundImage.color.WithAlpha(newValue);
        }

        private void OnBackgroundChanged()
        {
            if (string.IsNullOrEmpty(_backgroundName)) return;
            if (!BackgroundPackLoader.Backgrounds.TryGetValue(_backgroundName, out var newBackgrounds))
            {
                if (!BackgroundPackLoader.Backgrounds.TryGetValue("default", out newBackgrounds))
                {
                    _backgroundImage.texture = _defaultTexture;
                    return;
                }
            }

            var index = Random.Range(0, newBackgrounds.Count);
            var newBackground = newBackgrounds[index];
            switch (newBackground.Type)
            {
                case BackgroundRecord.BackgroundType.Video:
                    _videoPlayer.url = $"file://{newBackground.VideoPath}";
                    _backgroundImage.texture = _videoPlayer.targetTexture;
                    _videoPlayer.Play();
                    break;
                case BackgroundRecord.BackgroundType.Texture:
                    _backgroundImage.texture = newBackground.Texture;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}