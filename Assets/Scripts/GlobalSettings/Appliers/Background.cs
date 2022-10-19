using System;
using System.Collections.Generic;
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
        [SerializeField] private List<BackgroundRecord> _defaultBackgrounds;

        private RawImage _backgroundImage;
        private VideoPlayer _videoPlayer;
        private float _widthToHeightRatio = 16f / 9f;
        private float _heightToWidthRatio = 9f / 16f;
        private Camera _camera;
        private RectTransform _myTransform;
        private float _lastFrameRatio;

        private const float REFERENCE_WIDTH = 1920f;
        private const float REFERENCE_HEIGHT = 1080f;

        private void Awake()
        {
            _camera = FindObjectOfType<Camera>();
            _myTransform = GetComponent<RectTransform>();
            _backgroundImage = GetComponent<RawImage>();
            _videoPlayer = GetComponent<VideoPlayer>();
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
                    newBackgrounds = _defaultBackgrounds;
                }
            }

            if (newBackgrounds.Count == 0) return;

            var index = Random.Range(0, newBackgrounds.Count);
            var newBackground = newBackgrounds[index];
            Texture newTexture;
            switch (newBackground.Type)
            {
                case BackgroundType.Video:
                    _videoPlayer.url = $"file://{newBackground.VideoPath}";
                    newTexture = _videoPlayer.targetTexture;
                    _backgroundImage.texture = newTexture;
                    _videoPlayer.Play();
                    break;
                case BackgroundType.Texture:
                    newTexture = newBackground.Texture;
                    _backgroundImage.texture = newBackground.Texture;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (newTexture is null) return;

            _widthToHeightRatio = (float) newTexture.width / newTexture.height;
            _heightToWidthRatio = (float) newTexture.height / newTexture.width;

            UpdateBackgroundSize((float) _camera.pixelWidth / _camera.pixelHeight);
        }

        private void Update()
        {
            var realWidthToHeightRatio = (float) _camera.pixelWidth / _camera.pixelHeight;

            if (Math.Abs(realWidthToHeightRatio - _lastFrameRatio) < 0.000000001f)
                return;

            _lastFrameRatio = realWidthToHeightRatio;
            UpdateBackgroundSize(realWidthToHeightRatio);
        }

        private void UpdateBackgroundSize(float realRatio)
        {
            if (realRatio > _widthToHeightRatio)
            {
                var newWidth = REFERENCE_WIDTH * realRatio * _heightToWidthRatio;
                _myTransform.sizeDelta = new Vector2(
                    newWidth,
                    newWidth * _heightToWidthRatio);
            }
            else
            {
                _myTransform.sizeDelta = new Vector2(
                    REFERENCE_HEIGHT * _widthToHeightRatio,
                    REFERENCE_HEIGHT);
            }
        }
    }
}