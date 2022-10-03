using System;
using System.Collections.Generic;
using Blockstacker.GlobalSettings.Backgrounds;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Video;
using Random = UnityEngine.Random;

namespace Blockstacker.GlobalSettings.Appliers
{
    [RequireComponent(typeof(VideoPlayer))]
    public class UIDocumentBackground : MonoBehaviour
    {
        [SerializeField] private string _backgroundName;
        [SerializeField] private UIDocument _document;
        [SerializeField] private List<BackgroundRecord> _defaultBackgrounds;

        private VisualElement _rootElement;
        private VisualElement _backgroundElement;
        private VideoPlayer _videoPlayer;

        private void Awake()
        {
            _videoPlayer = GetComponent<VideoPlayer>();
        }

        private void OnEnable()
        {
            _rootElement = _document.rootVisualElement;
            _backgroundElement = _rootElement.Q("background");
            
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
            _backgroundElement.style.opacity = new StyleFloat(newValue);
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
            switch (newBackground.Type)
            {
                case BackgroundType.Video:
                    _videoPlayer.url = $"file://{newBackground.VideoPath}";
                    _backgroundElement.style.backgroundImage =
                        new StyleBackground(
                            UnityEngine.UIElements.Background.FromRenderTexture(_videoPlayer.targetTexture));
                    _videoPlayer.Play();
                    break;
                case BackgroundType.Texture:
                    _backgroundElement.style.backgroundImage = new StyleBackground(newBackground.Texture);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}