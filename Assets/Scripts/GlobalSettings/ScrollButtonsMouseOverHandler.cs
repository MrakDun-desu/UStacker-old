
/************************************
ScrollButtonsMouseOverHandler.cs -- created by Marek Dančo (xdanco00)
*************************************/
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UStacker.GlobalSettings
{
    public class ScrollButtonsMouseOverHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private float _openedSize;
        [SerializeField] private float _closedSize;
        [SerializeField] private RectTransform _controlledTransform;
        [SerializeField] private float _waitTime = 1;
        [SerializeField] private float _openTime = .2f;

        private bool _pointerOver;

        public void OnPointerEnter(PointerEventData eventData)
        {
            _pointerOver = true;
            StartCoroutine(OpenCoroutine());
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _pointerOver = false;
            DOTween.To(
                () => _controlledTransform.sizeDelta,
                value => _controlledTransform.sizeDelta = value,
                new Vector2(_closedSize, _controlledTransform.sizeDelta.y),
                _openTime);
        }

        private IEnumerator OpenCoroutine()
        {
            yield return new WaitForSeconds(_waitTime);
            if (!_pointerOver)
                yield break;

            DOTween.To(
                () => _controlledTransform.sizeDelta,
                value => _controlledTransform.sizeDelta = value,
                new Vector2(_openedSize, _controlledTransform.sizeDelta.y),
                _openTime);
        }
    }
}
/************************************
end ScrollButtonsMouseOverHandler.cs
*************************************/
