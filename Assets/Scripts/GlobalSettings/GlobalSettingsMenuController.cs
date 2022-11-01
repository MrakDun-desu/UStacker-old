using System;
using Blockstacker.Common.Extensions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Blockstacker.GlobalSettings
{
    public class GlobalSettingsMenuController : MonoBehaviour
    {
        [SerializeField] private RectTransform _menuTransform;
        [SerializeField] private float _openedX = 1000f;
        [SerializeField] private float _closedX = 300f;
        [SerializeField] private ScrollRect _settingsScrollRect;
        [SerializeField] private EventSystem _activeEventSystem;

        private GameObject _lastSelected;

        private bool _menuOpened;

        public void ToggleSettingsMenu()
        {
            if (_menuOpened)
                CloseSettingsMenu();
            else
                OpenSettingsMenu();
        }
        
        private void OpenSettingsMenu()
        {
            LeanTween.moveX(_menuTransform, _openedX, 0.5f).setEaseInOutSine();
            _menuOpened = true;
        }

        private void CloseSettingsMenu()
        {
            LeanTween.moveX(_menuTransform, _closedX, 0.5f).setEaseInOutSine();
            _menuOpened = false;
        }

        private void Update()
        {
            if (_lastSelected == _activeEventSystem.currentSelectedGameObject) return;

            _lastSelected = _activeEventSystem.currentSelectedGameObject;
            if (_activeEventSystem.currentSelectedGameObject == null) return;
            
            if (!_lastSelected.transform.IsChildOf(_settingsScrollRect.content)) return;

            _settingsScrollRect.ScrollTo((RectTransform)_lastSelected.transform, scrollSmooth: false);
        }

        public void ScrollTo(RectTransform target)
        {
            _settingsScrollRect.ScrollTo(target, minimalMovement: false);
        }
    }
}