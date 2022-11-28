using DG.Tweening;
using UnityEngine;

namespace Blockstacker.Common.UI
{
    public class OpenablePanel : MonoBehaviour
    {
        [SerializeField] private float _openedX = 1150f;
        [SerializeField] private float _closedX = 150f;
        [SerializeField] private float _tweenDuration = 0.5f;

        private bool _panelOpened;

        private RectTransform _controlledTransform => (RectTransform) transform;
        
        public void TogglePanel()
        {
            if (_panelOpened)
                ClosePanel();
            else
                OpenPanel();
        }
        
        public void OpenPanel()
        {
            if (_panelOpened) return;
            
            gameObject.SetActive(true);
            DOTween.To(GetPosX, SetPosX, _openedX, _tweenDuration);
            _panelOpened = true;
        }

        public void ClosePanel()
        {
            if (!_panelOpened) return;
            
            DOTween.To(GetPosX, SetPosX, _closedX, _tweenDuration).OnComplete(() => gameObject.SetActive(false));
            _panelOpened = false;
        }

        private float GetPosX()
        {
            return _controlledTransform.anchoredPosition.x;
        }

        private void SetPosX(float value)
        {
            var position = _controlledTransform.anchoredPosition;
            _controlledTransform.anchoredPosition = new Vector2(value, position.y);
        }

    }
}