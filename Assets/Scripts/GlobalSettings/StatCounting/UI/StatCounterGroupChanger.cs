using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace Blockstacker.GlobalSettings.StatCounting.UI
{
    public class StatCounterGroupChanger : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<StatCounterGroupChanger>
        {
        }

        private const string SELF_CLASS = "stat-counter-group-changer";
        private const string COUNTERS_CONTAINER_CLASS = "counters-container";
        private const string BUTTON_CONTAINER_CLASS = "button-container";
        private const string HEADER_CLASS = "group-header";
        private const string MINIMIZED_CLASS = "minimized";

        private const string REMOVE_BUTTON_TEXT = "Remove group";
        private const string ADD_BUTTON_TEXT = "Add stat counter";

        private readonly List<StatCounterChanger> _statCounterChangers = new();
        private StatCounterGroup _value = new();

        public StatCounterGroup Value
        {
            get => _value;
            set
            {
                _value = value;
                _nameField.SetValueWithoutNotify(value.Name);
                RefreshStatCounters();
            }
        }

        public Guid Id { get; set; }

        public Action<Guid> GroupRemoved;

        private readonly TextField _nameField;
        private readonly VisualElement _counterChangersContainer;
        private readonly StatCounterSO[] _premadeCounterTypes;

        public StatCounterGroupChanger() : this(Array.Empty<StatCounterSO>())
        {
        }

        public StatCounterGroupChanger(StatCounterSO[] premadeCounterTypes)
        {
            _premadeCounterTypes = premadeCounterTypes;

            var header = new VisualElement();
            header.AddToClassList(HEADER_CLASS);
            _nameField = new TextField("Group name");
            var minimizeButton = new Button(MinimizeButtonClicked) { text = string.Empty };
            header.Add(_nameField);
            header.Add(minimizeButton);

            _counterChangersContainer = new VisualElement();
            _counterChangersContainer.AddToClassList(COUNTERS_CONTAINER_CLASS);

            var buttonContainer = new VisualElement();
            buttonContainer.AddToClassList(BUTTON_CONTAINER_CLASS);
            var removeButton = new Button(() => GroupRemoved?.Invoke(Id)) {text = REMOVE_BUTTON_TEXT};
            var addButton = new Button(OnStatCounterAdded) {text = ADD_BUTTON_TEXT};
            buttonContainer.Add(removeButton);
            buttonContainer.Add(addButton);

            AddToClassList(SELF_CLASS);
            Add(header);
            Add(_counterChangersContainer);
            Add(buttonContainer);

            RefreshStatCounters();

            _nameField.RegisterValueChangedCallback(evt => OnNameChanged(evt.newValue));
        }

        private void RefreshStatCounters()
        {
            foreach (var counterChanger in _statCounterChangers)
            {
                counterChanger.Removed -= DeleteStatCounter;
                Remove(counterChanger);
            }

            _statCounterChangers.Clear();

            foreach (var counter in Value.StatCounters)
            {
                AddStatCounter(counter);
            }
        }

        private void MinimizeButtonClicked()
        {
            ToggleInClassList(MINIMIZED_CLASS);
        }

        private void AddStatCounter(StatCounterRecord newCounter, bool addToValue = false)
        {
            var newCounterChanger = new StatCounterChanger(_premadeCounterTypes)
            {
                Value = newCounter
            };
            newCounterChanger.Removed += DeleteStatCounter;
            _counterChangersContainer.Add(newCounterChanger);
            _statCounterChangers.Add(newCounterChanger);
            if (addToValue)
                Value.StatCounters.Add(newCounter);
        }

        private void DeleteStatCounter(StatCounterChanger changer)
        {
            if (!_statCounterChangers.Remove(changer)) return;

            _counterChangersContainer.Remove(changer);
            changer.Removed -= DeleteStatCounter;
            Value.StatCounters.Remove(changer.Value);
            _statCounterChangers.Remove(changer);
        }

        private void OnNameChanged(string newName)
        {
            Value.Name = newName;
        }

        private void OnStatCounterAdded() =>
            AddStatCounter(new StatCounterRecord(), true);
    }
}