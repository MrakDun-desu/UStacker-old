using Blockstacker.Common.Extensions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Blockstacker.Common
{
    [RequireComponent(typeof(ScrollRect))]
    public class ScrollRectScrollOnSelect : MonoBehaviour
    {
        [SerializeField] private bool _minimalMovement = true;
        private ScrollRect _scrollRect;
        private EventSystem _activeEventSystem;
        private GameObject _lastSelected;

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

            _scrollRect.ScrollTo((RectTransform)_lastSelected.transform, _minimalMovement, false);
        }
    }
}