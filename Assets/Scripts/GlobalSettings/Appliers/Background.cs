using Blockstacker.Common.Extensions;
using Blockstacker.GlobalSettings.Loaders;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

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
            if (BackgroundPackLoader.BackgroundImages.TryGetValue(_backgroundName, out var newImage))
                _backgroundImage.texture = newImage;
            else if (BackgroundPackLoader.BackgroundVideos.TryGetValue(_backgroundName, out var videoPath))
            {
                _backgroundImage.texture = _videoPlayer.targetTexture;
                _videoPlayer.url = "file://" + videoPath;
                _videoPlayer.Play();
            }
            else
                _backgroundImage.texture = _defaultTexture;
        }
    }
}