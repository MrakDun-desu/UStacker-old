using DG.Tweening;
using UnityEngine;

namespace Blockstacker.GlobalSettings
{
    public class SingleplayerGamesMenu : MonoBehaviour
    {
        
        [SerializeField] private RectTransform _menuTransform;
        [SerializeField] private float _openedX = 1000f;
        [SerializeField] private float _closedX = 300f;
        [SerializeField] private float _tweenDuration = 0.5f;

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
            gameObject.SetActive(true);
            DOTween.To(GetPosX, SetPosX, _openedX, _tweenDuration);
            _menuOpened = true;
        }

        public void CloseSettingsMenu()
        {
            DOTween.To(GetPosX, SetPosX, _closedX, _tweenDuration).OnComplete(() => gameObject.SetActive(false));
            _menuOpened = false;
        }

        private float GetPosX()
        {
            return _menuTransform.anchoredPosition.x;
        }

        private void SetPosX(float value)
        {
            var position = _menuTransform.anchoredPosition;
            _menuTransform.anchoredPosition = new Vector2(value, position.y);
        }
    }
}