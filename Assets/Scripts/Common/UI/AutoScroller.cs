
/************************************
AutoScroller.cs -- created by Marek Dančo (xdanco00)
*************************************/
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UStacker.Common.Extensions;

namespace UStacker.Common.UI
{
    [RequireComponent(typeof(ScrollRect))]
    public class AutoScroller : MonoBehaviour
    {
        [SerializeField] private bool _minimalMovement = true;
        private EventSystem _activeEventSystem;
        private ScrollRect _scrollRect;

        private void Awake()
        {
            _activeEventSystem = FindObjectOfType<EventSystem>();
            _scrollRect = GetComponent<ScrollRect>();
        }

        public void ScrollToElement(RectTransform target)
        {
            _scrollRect.ScrollTo(target, _minimalMovement, false);
        }

        public void ScrollToElementSmooth(RectTransform target)
        {
            _scrollRect.ScrollTo(target, _minimalMovement);
        }

        public void ScrollToSelected()
        {
            var current = _activeEventSystem.currentSelectedGameObject;

            if (current == null) return;
            var currentTransform = (RectTransform) current.transform;
            if (!currentTransform.IsChildOf(_scrollRect.content)) return;

            _scrollRect.ScrollTo(currentTransform, _minimalMovement, false);
        }

        public void ScrollLeft(float amount)
        {
            var contentPos = _scrollRect.content.localPosition;
            contentPos.x += amount;
            _scrollRect.content.anchoredPosition = contentPos;
        }

        public void ScrollRight(float amount)
        {
            var contentPos = _scrollRect.content.localPosition;
            contentPos.x -= amount;
            _scrollRect.content.anchoredPosition = contentPos;
        }

        public void ScrollUp(float amount)
        {
            var contentPos = _scrollRect.content.anchoredPosition;
            contentPos.y += amount;
            _scrollRect.content.anchoredPosition = contentPos;
        }

        public void ScrollDown(float amount)
        {
            var contentPos = _scrollRect.content.anchoredPosition;
            contentPos.y -= amount;
            _scrollRect.content.anchoredPosition = contentPos;
        }
    }
}
/************************************
end AutoScroller.cs
*************************************/
