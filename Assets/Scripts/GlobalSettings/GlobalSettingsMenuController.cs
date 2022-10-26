using UnityEngine;

namespace Blockstacker.GlobalSettings
{
    public class GlobalSettingsMenuController : MonoBehaviour
    {
        [SerializeField] private RectTransform _menuTransform;
        [SerializeField] private float _openedX = 1000f;
        [SerializeField] private float _closedX = 300f;
        
        private bool _menuOpened;

        public void ToggleSettingsMenu()
        {
            if (_menuOpened)
                CloseSettingsMenu();
            else
                OpenSettingsMenu();
        }
        
        public void OpenSettingsMenu()
        {
            LeanTween.moveX(_menuTransform, _openedX, 0.5f).setEaseInOutSine();
            _menuOpened = true;
        }

        public void CloseSettingsMenu()
        {
            LeanTween.moveX(_menuTransform, _closedX, 0.5f).setEaseInOutSine();
            _menuOpened = false;
        }
    }
}