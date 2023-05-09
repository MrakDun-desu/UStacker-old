
/************************************
OpenablePanel.cs -- created by Marek Dančo (xdanco00)
*************************************/
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

namespace UStacker.Common.UI
{
    public class OpenablePanel : MonoBehaviour
    {
        [SerializeField] private float _openedX = 1150f;
        [SerializeField] private float _closedX = 150f;
        [SerializeField] private float _tweenDuration = 0.5f;
        private TweenerCore<float, float, FloatOptions> _currentTween;

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
            _currentTween?.Kill();

            _currentTween = DOTween.To(GetPosX, SetPosX, _openedX, _tweenDuration).OnKill(NullTween);
            _panelOpened = true;
        }

        public void ClosePanel()
        {
            if (!_panelOpened) return;

            _currentTween?.Kill();

            _currentTween = DOTween.To(GetPosX, SetPosX, _closedX, _tweenDuration)
                .OnComplete(DeactivateGameObject).OnKill(NullTween);
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

        private void DeactivateGameObject()
        {
            gameObject.SetActive(false);
        }

        private void NullTween()
        {
            _currentTween = null;
        }
    }
}
/************************************
end OpenablePanel.cs
*************************************/
