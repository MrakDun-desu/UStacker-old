using Blockstacker.Loaders;
using UnityEngine;
using UnityEngine.UI;

namespace Blockstacker.GlobalSettings.Appliers
{
    [RequireComponent(typeof(RawImage))]
    public class Background : MonoBehaviour
    {
        [SerializeField] private string _backgroundName;

        private RawImage _backgroundImage;
        private Texture _defaultTexture;

        private void Awake()
        {
            _backgroundImage = GetComponent<RawImage>();
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
            var formerColor = _backgroundImage.color;
            _backgroundImage.color = new Color(formerColor.r, formerColor.g, formerColor.b, newValue);
        }

        private void OnBackgroundChanged()
        {
            if (string.IsNullOrEmpty(_backgroundName)) return;
            if (BackgroundPackLoader.Backgrounds.TryGetValue(_backgroundName, out var newImage)) {
                _backgroundImage.texture = newImage;
            }
            else {
                _backgroundImage.texture = _defaultTexture;
            }
        }
    }
}