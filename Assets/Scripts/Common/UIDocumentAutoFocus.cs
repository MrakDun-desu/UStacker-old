using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace Blockstacker.Common
{
    [RequireComponent(typeof(UIDocument))]
    public class UIDocumentAutoFocus : MonoBehaviour
    {
        [SerializeField] private UIDocument _document;

        private const string FIRST_FOCUSED_NAME = "firstFocused";
        private VisualElement _root;
        private VisualElement _firstFocused;
        private bool _hasStarted;

        private void Start()
        {
            // if not ran in start, for some reason doesn't do anything
            _firstFocused?.Focus();
            _hasStarted = true;
        }

        private void OnEnable()
        {
            // things that run in Awake normally need to run in OnEnable with UI Toolkit
            _root = _document.rootVisualElement;

            if ((_firstFocused = _root.Q(FIRST_FOCUSED_NAME)) is null)
            {
                var allElements = _root.Query().Build().ToList();
                _firstFocused = allElements.Find(el => el.canGrabFocus);
            }

            if (_hasStarted)
                _firstFocused.Focus();
        }
    }
}