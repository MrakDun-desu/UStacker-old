using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Blockstacker.GlobalSettings
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
            LeanTween.size(
                _controlledTransform, 
                new Vector2(_closedSize, _controlledTransform.sizeDelta.y), 
                _openTime)
                .setEaseInOutSine();
            var position = _controlledTransform.localPosition;
            LeanTween.move(
                _controlledTransform,
                new Vector3(_closedSize / 2f, position.y, position.z),
                _openTime)
                .setEaseInOutSine();
        }

        private IEnumerator OpenCoroutine()
        {
            yield return new WaitForSeconds(_waitTime);
            if (!_pointerOver)
                yield break;
            
            LeanTween.size(
                _controlledTransform, 
                new Vector2(_openedSize, _controlledTransform.sizeDelta.y), 
                _openTime)
                .setEaseInOutSine();
            var position = _controlledTransform.localPosition;
            LeanTween.move(
                _controlledTransform,
                new Vector3(_openedSize / 2f, position.y, position.z),
                _openTime)
                .setEaseInOutSine();
        }
    }
}