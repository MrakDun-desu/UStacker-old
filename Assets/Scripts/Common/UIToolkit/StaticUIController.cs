using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace Blockstacker.Common.UIToolkit
{
    public class StaticUIController : MonoBehaviour
    {
        [SerializeField] private UIDocument _menuDocument;
        [SerializeField] private List<UIElementBinding> _elementBindings;
        
        private void OnEnable()
        {
            var root = _menuDocument.rootVisualElement;

            foreach (var elementBinding in _elementBindings)
            {
                var element = root.Q(elementBinding.ControlName);
                switch (element)
                {
                    case Button btn:
                        btn.SetEnabled(elementBinding.IsEnabled);
                        btn.clicked += elementBinding.BoundAction.Invoke;
                        break;
                    case INotifyOnChange notifyOnChange:
                        element.SetEnabled(elementBinding.IsEnabled);
                        notifyOnChange.Changed += elementBinding.BoundAction.Invoke;
                        break;
                }
            }
        }

        [ContextMenu("Search for bindings")]
        private void FindBindableElements()
        {
            var root = _menuDocument.rootVisualElement;
            var bindableElements = root.Query().Build().ToList();

            foreach (var element in bindableElements.Where(element => element is Button or INotifyOnChange))
            {
                _elementBindings.Add(new UIElementBinding(element.name));
            }
        }
    }
}