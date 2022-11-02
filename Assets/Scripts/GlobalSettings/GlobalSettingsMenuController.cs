using Blockstacker.Common.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace Blockstacker.GlobalSettings
{
    public class GlobalSettingsMenuController : MonoBehaviour
    {
        [SerializeField] private RectTransform _menuTransform;
        [SerializeField] private float _openedX = 1000f;
        [SerializeField] private float _closedX = 300f;
        [SerializeField] private ScrollRect _settingsScrollRect;

        private bool _menuOpened;
        private bool _loaded;

        private void LateUpdate()
        {
            if (_loaded) return;
            gameObject.SetActive(false);
            _loaded = true;
        }

        public void ToggleSettingsMenu()
        {
            if (_menuOpened)
                CloseSettingsMenu();
            else
                OpenSettingsMenu();
        }
        
        private void OpenSettingsMenu()
        {
            gameObject.SetActive(true);
            LeanTween.moveX(_menuTransform, _openedX, 0.5f).setEaseInOutSine();
            _menuOpened = true;
        }

        private void CloseSettingsMenu()
        {
            LeanTween.moveX(_menuTransform, _closedX, 0.5f).setEaseInOutSine().setOnComplete(() => gameObject.SetActive(false));
            _menuOpened = false;
        }

        public void ScrollTo(RectTransform target)
        {
            _settingsScrollRect.ScrollTo(target, minimalMovement: false);
        }
    }
}