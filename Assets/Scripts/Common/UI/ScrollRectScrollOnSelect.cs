
/************************************
ScrollRectScrollOnSelect.cs -- created by Marek Dančo (xdanco00)
*************************************/
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UStacker.Common.Extensions;

namespace UStacker.Common.UI
{
    [RequireComponent(typeof(ScrollRect))]
    public class ScrollRectScrollOnSelect : MonoBehaviour
    {
        [SerializeField] private bool _minimalMovement = true;
        private EventSystem _activeEventSystem;
        private GameObject _lastSelected;
        private ScrollRect _scrollRect;

        private void Awake()
        {
            _activeEventSystem = FindObjectOfType<EventSystem>();
            _scrollRect = GetComponent<ScrollRect>();
        }

        private void Update()
        {
            if (_lastSelected == _activeEventSystem.currentSelectedGameObject) return;

            _lastSelected = _activeEventSystem.currentSelectedGameObject;
            if (_lastSelected == null) return;

            if (!_lastSelected.transform.IsChildOf(_scrollRect.content)) return;

            _scrollRect.ScrollTo((RectTransform) _lastSelected.transform, _minimalMovement, false);
        }
    }
}
/************************************
end ScrollRectScrollOnSelect.cs
*************************************/
