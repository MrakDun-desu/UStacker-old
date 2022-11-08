using Blockstacker.Common.Extensions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Blockstacker.Common
{
    [RequireComponent(typeof(ScrollRect))]
    public class AutoScroller : MonoBehaviour
    {
        [SerializeField] private bool _minimalMovement = true;
        private ScrollRect _scrollRect;
        private EventSystem _activeEventSystem;

        private void Awake()
        {
            _activeEventSystem = FindObjectOfType<EventSystem>();
            _scrollRect = GetComponent<ScrollRect>();
        }

        public void ScrollToElement(RectTransform target)
        {
            _scrollRect.ScrollTo(target, _minimalMovement, false);
        }

        public void ScrollToSelected()
        {
            var current = _activeEventSystem.currentSelectedGameObject;

            if (current == null) return;
            
            _scrollRect.ScrollTo((RectTransform)current.transform, _minimalMovement, false);
        }
        
        public void ScrollLeft(float amount)
        {
            var contentPos = _scrollRect.content.localPosition;
            contentPos.x += amount;
            LeanTween.move(_scrollRect.content, contentPos, .2f).setEaseInOutSine();
        }
        
        public void ScrollRight(float amount)
        {
            var contentPos = _scrollRect.content.localPosition;
            contentPos.x -= amount;
            LeanTween.move(_scrollRect.content, contentPos, .2f).setEaseInOutSine();
        }
        
        public void ScrollUp(float amount)
        {
            var contentPos = _scrollRect.content.anchoredPosition;
            contentPos = new Vector2(contentPos.x, contentPos.y + amount);
            _scrollRect.content.anchoredPosition = contentPos;
        }
        
        public void ScrollDown(float amount)
        {
            var contentPos = _scrollRect.content.anchoredPosition;
            contentPos = new Vector2(contentPos.x, contentPos.y - amount);
            _scrollRect.content.anchoredPosition = contentPos;
        }
    }
}