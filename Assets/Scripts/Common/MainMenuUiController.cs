using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Blockstacker.Common
{
    public class MainMenuUiController : MonoBehaviour
    {
        [SerializeField] private UIDocument _menuDocument;
        [SerializeField] private List<UiElementsButtonBinding> _buttonBindings;
        
        private void OnEnable()
        {
            var root = _menuDocument.rootVisualElement;

            foreach (var buttonBinding in _buttonBindings)
            {
                var button = root.Q<Button>(buttonBinding.ButtonName);
                if (button is null) continue;
                
                button.SetEnabled(buttonBinding.IsEnabled);
                button.clicked += buttonBinding.OnClick.Invoke;
            }
        }

        [ContextMenu("Find buttons")]
        private void SearchForButtons()
        {
            var root = _menuDocument.rootVisualElement;
            var buttons = root.Query<Button>().Build().ToList();

            foreach (var button in buttons)
            {
                _buttonBindings.Add(
                    new UiElementsButtonBinding(button.name));
            }
        }
    }
}