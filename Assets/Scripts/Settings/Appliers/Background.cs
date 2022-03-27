using Blockstacker.Loaders;
using UnityEngine;
using UnityEngine.UI;

namespace Blockstacker.Settings.Appliers
{
    [RequireComponent(typeof(RawImage))]
    public class Background : MonoBehaviour
    {
        [SerializeField] private string _backgroundName;

        private RawImage _backgroundImage;

        private void Awake()
        {
            _backgroundImage = GetComponent<RawImage>();
        }

        private void OnEnable()
        {
            OnVisibilityChanged(AppSettings.Video.BackgroundVisibility);
            OnBackgroundChanged(_backgroundName);
            BackgroundVisibilityApplier.VisibilityChanged += OnVisibilityChanged;
            BackgroundPackLoader.BackgroundChanged += OnBackgroundChanged;
        }

        private void OnDisable()
        {
            BackgroundVisibilityApplier.VisibilityChanged -= OnVisibilityChanged;
            BackgroundPackLoader.BackgroundChanged -= OnBackgroundChanged;
        }

        private void OnVisibilityChanged(float newValue)
        {
            var newColor = _backgroundImage.color;
            _backgroundImage.color = new Color(newColor.r, newColor.g, newColor.b, newValue);
        }

        private void OnBackgroundChanged(string name)
        {
            if (!name.Equals(_backgroundName) ||
                !BackgroundPackLoader.Backgrounds.ContainsKey(_backgroundName))
                return;
            var newImage = BackgroundPackLoader.Backgrounds[_backgroundName];
            _backgroundImage.texture = newImage;
        }
    }
}