using System;
using System.Collections.Generic;
using Blockstacker.GlobalSettings.StatCounting;
using JetBrains.Annotations;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Blockstacker.GlobalSettings.Changers
{
    public class StatCounterGroupChanger : MonoBehaviour
    {
        [SerializeField] private TMP_InputField _nameField;
        [SerializeField] private Button _minimizeButton;
        [SerializeField] private TMP_Text _minimizeButtonText;
        [SerializeField] private Button _counterAddButton;
        [SerializeField] private Button _groupRemoveButton;
        [SerializeField] private RectTransform _counterChangersContainer;
        [SerializeField] private StatCounterChanger _counterChangerPrefab;

        public StatCounterGroup Value
        {
            get => _value;
            set
            {
                _value = value;
                _nameField.SetTextWithoutNotify(value.Name);
                RefreshStatCounters();
            }
        }

        public Guid Id { get; set; }
        public event Action<Guid> GroupRemoved;
        public event Action<float> SizeChanged;

        private StatCounterGroup _value;
        private RectTransform _selfTransform;

        private float _formerHeight;
        private bool _isMinimized;
        private readonly List<StatCounterChanger> _statCounterChangers = new();

        private void Awake()
        {
            _selfTransform = GetComponent<RectTransform>();

            _nameField.SetTextWithoutNotify("Group name");
            _nameField.onValueChanged.AddListener(OnNameChanged);
            _minimizeButton.onClick.AddListener(OnMinimize);
            _groupRemoveButton.onClick.AddListener(() => GroupRemoved?.Invoke(Id));
            _counterAddButton.onClick.AddListener(OnStatCounterAdded);

            SizeChanged += ChangeHeight;
        }

        private void ChangeHeight(float sizeUpdate) {
            _selfTransform.sizeDelta = new Vector2(
                _selfTransform.sizeDelta.x,
                _selfTransform.sizeDelta.y + sizeUpdate
            );

            _counterChangersContainer.position = new Vector2(
                _counterChangersContainer.position.y,
                _counterChangersContainer.sizeDelta.y / 2f
            );
        }

        private void RefreshStatCounters()
        {
            foreach (var counterChanger in _statCounterChangers)
            {
                counterChanger.Removed -= DeleteStatCounter;
                Destroy(counterChanger.gameObject);
            }

            _statCounterChangers.Clear();

            foreach (var counter in Value.StatCounters)
                AddStatCounter(counter);
        }

        private void OnMinimize() {
            if (_isMinimized) {
                _counterChangersContainer.sizeDelta = new Vector2(
                    _counterChangersContainer.sizeDelta.x, _formerHeight
                );
            } else {
                _formerHeight = _counterChangersContainer.sizeDelta.y;
                _counterChangersContainer.sizeDelta = new Vector2(
                    _counterChangersContainer.sizeDelta.x, 0
                );
            }

            _isMinimized = !_isMinimized;
            _minimizeButtonText.text = _isMinimized ? "+" : "-";
        }

        private void OnNameChanged(string newValue)
        {
            Value.Name = newValue;
        }

        private void AddStatCounter(StatCounterRecord newCounter, bool addToValue = false)
        {
            var newCounterChanger = Instantiate(_counterChangerPrefab, _counterChangersContainer);
            newCounterChanger.Value = newCounter;

            var addedSize = newCounterChanger.GetComponent<RectTransform>().sizeDelta.y;

            SizeChanged?.Invoke(addedSize);

            newCounterChanger.Removed += DeleteStatCounter;
            _statCounterChangers.Add(newCounterChanger);
            if (addToValue)
                Value.StatCounters.Add(newCounter);
        }

        private void DeleteStatCounter(StatCounterChanger changer)
        {
            if (!_statCounterChangers.Remove(changer)) return;

            var reducedSize = changer.GetComponent<RectTransform>().sizeDelta.y;
            SizeChanged?.Invoke(-reducedSize);

            Destroy(changer.gameObject);
            changer.Removed -= DeleteStatCounter;
            Value.StatCounters.Remove(changer.Value);
            _statCounterChangers.Remove(changer);
        }

        private void OnStatCounterAdded() => AddStatCounter(new StatCounterRecord(), true);
    }
}