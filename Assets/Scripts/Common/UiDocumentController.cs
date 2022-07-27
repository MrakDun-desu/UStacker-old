using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace Blockstacker.Common
{
    [RequireComponent(typeof(UIDocument))]
    public class UiDocumentController : MonoBehaviour
    {
        [SerializeField] private UIDocument _document;
        [SerializeField] private ButtonDictionary _buttonEvents;

        private const string FIRST_FOCUSED_NAME = "first-focused";
        private VisualElement _root;
        private VisualElement _firstFocused;
        private bool _hasStarted;

        [ContextMenu("Search for buttons")]
        private void SearchForButtons()
        {
            _buttonEvents.Clear();
            
            if (_document == null)
                return;

            _root = _document.rootVisualElement;

            _root.Query<Button>().ForEach(button => { _buttonEvents.Add(button.name, new UnityEvent()); });
        }

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

            foreach (var buttonEvent in _buttonEvents)
            {
                _root.Q<Button>(buttonEvent.Key).clicked += buttonEvent.Value.Invoke;
            }
            if (_hasStarted)
                _firstFocused.Focus();
        }
    }
}