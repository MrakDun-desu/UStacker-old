using System;
using System.Collections.Generic;
using UStacker.Common.Extensions;
using UStacker.GlobalSettings.Backgrounds;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using Random = UnityEngine.Random;

namespace UStacker.GlobalSettings.Appliers
{
    [RequireComponent(typeof(RawImage), typeof(VideoPlayer))]
    public class Background : MonoBehaviour
    {

        private const float REFERENCE_HEIGHT = 1080f;
        [SerializeField] private string _backgroundName;
        [SerializeField] private List<BackgroundRecord> _defaultBackgrounds;

        private RawImage _backgroundImage;
        private Camera _camera;
        private float _heightToWidthRatio = 9f / 16f;
        private float _lastFrameRatio;
        private RectTransform _myTransform;
        private float _textureWidth;
        private VideoPlayer _videoPlayer;
        private float _widthToHeightRatio = 16f / 9f;

        private void Awake()
        {
            _camera = FindObjectOfType<Camera>();
            _myTransform = GetComponent<RectTransform>();
            _backgroundImage = GetComponent<RawImage>();
            _videoPlayer = GetComponent<VideoPlayer>();
        }

        private void Update()
        {
            var pixelHeight = _camera.pixelHeight;
            var realWidthToHeightRatio = (float) _camera.pixelWidth / pixelHeight;

            if (Math.Abs(realWidthToHeightRatio - _lastFrameRatio) < float.Epsilon)
                return;

            _lastFrameRatio = realWidthToHeightRatio;
            UpdateBackgroundSize(realWidthToHeightRatio);
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
                if (!BackgroundPackLoader.Backgrounds.TryGetValue("default", out newBackgrounds)) newBackgrounds = _defaultBackgrounds;
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

            _textureWidth = newTexture.width;
            _widthToHeightRatio = (float) newTexture.width / newTexture.height;
            _heightToWidthRatio = (float) newTexture.height / newTexture.width;

            UpdateBackgroundSize((float) _camera.pixelWidth / _camera.pixelHeight);
        }

        private void UpdateBackgroundSize(float realRatio)
        {
            if (realRatio > _widthToHeightRatio)
            {
                var realWidth = _camera.pixelWidth * REFERENCE_HEIGHT / _camera.pixelHeight;
                _myTransform.sizeDelta = new Vector2(
                    realWidth,
                    realWidth * _heightToWidthRatio);
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