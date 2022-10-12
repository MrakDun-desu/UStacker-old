using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Blockstacker.Common.Extensions;
using UnityEngine;
using UnityEngine.UIElements;

namespace Blockstacker.Common.UIToolkit
{
    public class BsDropdownField : BaseField<string>
    {
        public new class UxmlTraits : BaseField<string>.UxmlTraits
        {
            private readonly UxmlStringAttributeDescription _choicesString = new() {name = "choices-string"};
            private readonly UxmlBoolAttributeDescription _searchable = new() {name = "searchable"};

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);

                if (ve is not BsDropdownField field) return;

                field.Searchable = _searchable.GetValueFromBag(bag, cc);
                field.ChoicesString = _choicesString.GetValueFromBag(bag, cc);
            }
        }

        public new class UxmlFactory : UxmlFactory<BsDropdownField, UxmlTraits>
        {
        }

        protected string ChoicesString
        {
            get => string.Join(',', Choices.Select(choice => choice.Value));
            set
            {
                foreach (var choiceStr in value.Split(','))
                {
                    if (Choices.All(choice => choice.Value != choiceStr))
                        Choices.Add(new ChoiceWithTooltip(choiceStr));
                }
            }
        }

        protected bool Searchable
        {
            get => _searchable;
            set
            {
                _searchable = value;
                _searchField.textEdition.isReadOnly = !_searchable;
            }
        }

        public ObservableCollection<ChoiceWithTooltip> Choices { get; } = new();

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
                this.value = Choices[_index].Value;
                _searchField.SetValueWithoutNotify(Choices[_index].Value);
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

        private bool _searchable;
        private int _focusedIndex = -1;
        private int _index;
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
            _choicesView = new ScrollView {style = {display = DisplayStyle.None}};

            visualInput.Add(_searchField);
            visualInput.Add(_choicesView);
            _searchField.AddToClassList(INPUT_CLASS);
            _searchField.isDelayed = false;

            AddToClassList(SELF_CLASS);

            _searchField.RegisterCallback<FocusInEvent>(OnFieldFocused);
            _searchField.RegisterCallback<FocusOutEvent>(OnFieldUnfocused);
            _searchField.RegisterCallback<KeyDownEvent>(OnKeyPressed);
            _searchField.RegisterValueChangedCallback(FilterChoices);
            Choices.CollectionChanged += (_, _) => OnChoicesChanged();
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
                var newDropdownChoice = new DropdownChoice(choice.Value, i);
                newDropdownChoice.Container.AddToClassList(CHOICE_CLASS);
                newDropdownChoice.Container.tooltip = choice.Tooltip;
                newDropdownChoice.Container.RegisterCallback<MouseDownEvent>(_ =>
                    OnChoiceClicked(newDropdownChoice.Index));

                _choicesView.Add(newDropdownChoice.Container);
                _dropdownChoices.Add(newDropdownChoice);
            }

            Index = 0;
        }

        private void OnChoiceClicked(int index)
        {
            Index = index;
            FocusedIndex = index;
        }

        private void OnKeyPressed(KeyDownEvent evt)
        {
            switch (evt.keyCode)
            {
                case KeyCode.Escape:
                    _choicesView.style.display = DisplayStyle.None;
                    break;
                case KeyCode.KeypadEnter or KeyCode.Return:
                    Index = FocusedIndex;
                    Focus();
                    break;
                case KeyCode.DownArrow:
                    IncreaseFocusIndex();
                    break;
                case KeyCode.UpArrow:
                    DecreaseFocusIndex();
                    break;
            }
        }

        private void IncreaseFocusIndex()
        {
            do
            {
                FocusedIndex++;
            } while (!_dropdownChoices[FocusedIndex].Enabled);
        }

        private void DecreaseFocusIndex()
        {
            do
            {
                FocusedIndex--;
            } while (!_dropdownChoices[FocusedIndex].Enabled);
        }

        private void FilterChoices(ChangeEvent<string> evt)
        {
            var searchText = evt.newValue.RemoveDiacritics().ToLowerInvariant();
            if (string.IsNullOrEmpty(searchText))
            {
                foreach (var choice in _dropdownChoices)
                {
                    choice.Label.text = choice.Name;
                    choice.Enabled = true;
                }

                return;
            }

            foreach (var choice in _dropdownChoices)
            {
                var normalizedChoiceName = choice.Name.RemoveDiacritics().ToLowerInvariant();
                var foundIndices = new List<int>();

                for (var searchStart = 0; searchStart >= 0;)
                {
                    searchStart = normalizedChoiceName.IndexOf(searchText, searchStart, StringComparison.Ordinal);
                    if (searchStart < 0) continue;
                    foundIndices.Add(searchStart);
                    searchStart++;
                }

                choice.Enabled = foundIndices.Count > 0;

                if (foundIndices.Count == 0)
                {
                    continue;
                }

                var newChoiceText = choice.Name;
                for (var i = foundIndices.Count - 1; i >= 0; i--)
                {
                    var foundStart = foundIndices[i];
                    var foundEnd = foundStart + searchText.Length;

                    newChoiceText = newChoiceText.Insert(foundEnd, "</color>");
                    newChoiceText = newChoiceText.Insert(foundStart, "<color=#ffe44d>");
                }

                choice.Label.text = newChoiceText;
            }
        }

        private void OnFieldFocused(FocusInEvent evt)
        {
            if (Searchable)
                _searchField.value = string.Empty;

            _choicesView.style.display = DisplayStyle.Flex;
        }

        private void OnFieldUnfocused(FocusOutEvent evt)
        {
            _choicesView.style.display = DisplayStyle.None;
            _searchField.SetValueWithoutNotify(value);
        }

        public override void SetValueWithoutNotify(string newValue)
        {
            if (Choices.All(choice => choice.Value != newValue)) return;

            base.SetValueWithoutNotify(newValue);
            _searchField.SetValueWithoutNotify(newValue);
        }

        private class DropdownChoice
        {
            public DropdownChoice(string name, int index)
            {
                Name = name;
                Label = new Label(Name);
                Container = new VisualElement();
                Container.Add(Label);
                Index = index;
            }

            public string Name { get; }
            public Label Label { get; }
            public VisualElement Container { get; }
            public int Index { get; }

            public bool Enabled
            {
                get => _enabled;
                set
                {
                    _enabled = value;
                    Container.style.display = _enabled ? DisplayStyle.Flex : DisplayStyle.None;
                }
            }

            private bool _enabled = true;
        }

        public class ChoiceWithTooltip
        {
            public string Value { get; }
            public string Tooltip { get; }

            public ChoiceWithTooltip(string value, string tooltip = "")
            {
                Value = value;
                Tooltip = tooltip;
            }
        }
    }
}