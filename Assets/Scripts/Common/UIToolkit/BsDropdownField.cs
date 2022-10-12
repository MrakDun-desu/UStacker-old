using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.UIElements;

namespace Blockstacker.Common.UIToolkit
{
    public class BsDropdownField : BaseField<string>
    {
        public new class UxmlTraits : BaseField<string>.UxmlTraits
        {
            private readonly UxmlStringAttributeDescription _choicesString = new()
            {
                name = "choices-string"
            };

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);

                if (ve is not BsDropdownField field) return;

                field.ChoicesString = _choicesString.GetValueFromBag(bag, cc);
            }
        }

        public new class UxmlFactory : UxmlFactory<BsDropdownField, UxmlTraits>
        {
        }

        protected string ChoicesString
        {
            get => _choicesString;
            set
            {
                _choicesString = value;
                Choices.Clear();
                foreach (var choice in _choicesString.Split (','))
                {
                    Choices.Add(choice);
                }
                
            }
        }

        public ObservableCollection<string> Choices { get; } = new();

        public int Index
        {
            get => _index;
            set
            {
                if (Choices.Count == 0)
                {
                    _index = 0;
                    return;
                }
                
                _dropdownChoices[_index].Container.RemoveFromClassList(SELECTED_CLASS);
                
                _index = Mathf.Clamp(value, 0, Choices.Count - 1);
                
                _dropdownChoices[_index].Container.AddToClassList(SELECTED_CLASS);
                this.value = Choices[_index];
            }
        }

        private int FocusedIndex
        {
            get => _focusedIndex;
            set
            {
                if (_dropdownChoices.Count == 0)
                {
                    _focusedIndex = 0;
                    return;
                }

                if (_focusedIndex < _dropdownChoices.Count && _focusedIndex >= 0)
                    _dropdownChoices[_focusedIndex].Container.RemoveFromClassList(FOCUSED_CLASS);

                _focusedIndex = value % _dropdownChoices.Count;
                if (_focusedIndex < 0)
                    _focusedIndex += _dropdownChoices.Count;
                
                _dropdownChoices[_focusedIndex].Container.AddToClassList(FOCUSED_CLASS);
            }
        }

        private int _focusedIndex = -1;
        private int _index;
        private string _choicesString;
        private readonly List<DropdownChoice> _dropdownChoices = new();
        private readonly ScrollView _choicesView;
        private readonly TextField _searchField;

        private const string SELF_CLASS = "bs-dropdown";
        private const string INPUT_CLASS = "dropdown-input";
        private const string CHOICE_CLASS = "dropdown-choice";
        private const string SELECTED_CLASS = "selected";
        private const string FOCUSED_CLASS = "focused";

        private BsDropdownField(string label, VisualElement visualInput) : base(label, visualInput)
        {

            _searchField = new TextField();
            _choicesView = new ScrollView { style = { display = DisplayStyle.None } };

            visualInput.Add(_searchField);
            visualInput.Add(_choicesView);
            _searchField.AddToClassList(INPUT_CLASS);
            _searchField.isDelayed = true;
            
            AddToClassList(SELF_CLASS);
            
            _searchField.RegisterCallback<FocusInEvent>(OnFieldFocused);
            _searchField.RegisterCallback<FocusOutEvent>(OnFieldUnfocused);
            Choices.CollectionChanged += (_, _) => OnChoicesChanged();
            RegisterCallback<KeyDownEvent>(OnKeyPressed);
            this.RegisterValueChangedCallback(OnValueChanged);
        }

        public BsDropdownField() : this("Label", new VisualElement())
        {
        }

        private void OnChoicesChanged()
        {
            foreach (var choice in _dropdownChoices)
                _choicesView.Remove(choice.Container);
            
            _dropdownChoices.Clear();
            
            for (var i = 0; i < Choices.Count; i++)
            {
                var choice = Choices[i];
                var newDropdownChoice = new DropdownChoice(new Label(choice), new VisualElement(), i);
                newDropdownChoice.Container.AddToClassList(CHOICE_CLASS);
                newDropdownChoice.Container.Add(newDropdownChoice.Label);
                newDropdownChoice.Container.RegisterCallback<MouseDownEvent>(_ => OnChoiceClicked(newDropdownChoice.Index));
                
                _choicesView.Add(newDropdownChoice.Container);
                _dropdownChoices.Add(newDropdownChoice);
            }
        }

        private void OnChoiceClicked(int index)
        {
            Index = index;
        }

        private void OnKeyPressed(KeyDownEvent evt)
        {
            switch (evt.keyCode)
            {
                case KeyCode.Escape:
                    _choicesView.style.display = DisplayStyle.None;
                    _searchField.value = value;
                    break;
                case KeyCode.KeypadEnter or KeyCode.Return:
                    Index = FocusedIndex;
                    _searchField.value = value;
                    _choicesView.style.display = DisplayStyle.None;
                    evt.PreventDefault();
                    break;
                case KeyCode.DownArrow:
                    FocusedIndex++;
                    break;
                case KeyCode.UpArrow:
                    FocusedIndex = _focusedIndex < 0 ? _dropdownChoices.Count - 1 : FocusedIndex--;
                    break;
            }
        }

        private void OnValueChanged(ChangeEvent<string> evt)
        {
            _searchField.value = evt.newValue;
        }
        
        private void OnFieldFocused(FocusInEvent evt)
        {
            _searchField.value = string.Empty;
            _choicesView.style.display = DisplayStyle.Flex;
        }
        
        private void OnFieldUnfocused(FocusOutEvent evt)
        {
            _searchField.value = value;
            _choicesView.style.display = DisplayStyle.None;
        }


        private class DropdownChoice
        {
            public DropdownChoice(Label label, VisualElement container, int index)
            {
                Label = label;
                Container = container;
                Index = index;
            }

            public Label Label { get; }
            public VisualElement Container { get; }
            public int Index { get; }
            
            
        }
    }
}