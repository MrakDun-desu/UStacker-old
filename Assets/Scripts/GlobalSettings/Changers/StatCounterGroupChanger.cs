using System;
using System.Collections.Generic;
using UStacker.GlobalSettings.StatCounting;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UStacker.GlobalSettings.Changers
{
    public class StatCounterGroupChanger : MonoBehaviour
    {
        [SerializeField] private TMP_InputField _nameField;
        [SerializeField] private Button _minimizeButton;
        [SerializeField] private TMP_Text _minimizeButtonText;
        [SerializeField] private Button _counterAddButton;
        [SerializeField] private Button _groupRemoveButton;
        [SerializeField] private RectTransform _counterChangersContainer;
        [SerializeField] private ContentSizeFitter _containerSizeFitter;
        [SerializeField] private StatCounterChanger _counterChangerPrefab;
        [SerializeField] private float _statCounterSpacing = 10;
        private readonly List<StatCounterChanger> _statCounterChangers = new();

        private float _formerHeight;
        private bool _isMinimized;
        private RectTransform _selfTransform;

        private StatCounterGroup _value;

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

        private void Awake()
        {
            _selfTransform = transform as RectTransform;
            SizeChanged += ChangeHeight;
            _nameField.SetTextWithoutNotify("Group name");
        }

        private void Start()
        {
            _nameField.onValueChanged.AddListener(OnNameChanged);
            _minimizeButton.onClick.AddListener(OnMinimize);
            _groupRemoveButton.onClick.AddListener(() => GroupRemoved?.Invoke(Id));
            _counterAddButton.onClick.AddListener(OnStatCounterAdded);
        }

        public event Action<Guid> GroupRemoved;
        public event Action<float> SizeChanged;

        private void ChangeHeight(float sizeDelta)
        {
            var selfSizeDelta = _selfTransform.sizeDelta;
            selfSizeDelta = new Vector2(
                selfSizeDelta.x,
                selfSizeDelta.y + sizeDelta
            );
            _selfTransform.sizeDelta = selfSizeDelta;

            var counterChangersPos = _counterChangersContainer.anchoredPosition;
            counterChangersPos = new Vector2(
                counterChangersPos.x,
                counterChangersPos.y - sizeDelta / 2f
            );
            _counterChangersContainer.anchoredPosition = counterChangersPos;
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

        private void OnMinimize()
        {
            _containerSizeFitter.enabled = _isMinimized;
            if (_isMinimized)
            {
                _counterChangersContainer.sizeDelta = new Vector2(
                    _counterChangersContainer.sizeDelta.x, _formerHeight
                );
            }
            else
            {
                var containerSize = _counterChangersContainer.sizeDelta;

                _formerHeight = containerSize.y;
                containerSize = new Vector2(
                    containerSize.x, 0
                );
                _counterChangersContainer.sizeDelta = containerSize;
            }

            _counterChangersContainer.gameObject.SetActive(_isMinimized);
            _isMinimized = !_isMinimized;
            _minimizeButtonText.text = _isMinimized ? "+" : "-";
            SizeChanged?.Invoke(_formerHeight * (_isMinimized ? -1 : 1));
        }

        private void OnNameChanged(string newValue)
        {
            Value.Name = newValue;
        }

        private void AddStatCounter(StatCounterRecord newCounter, bool addToValue = false)
        {
            var newCounterChanger = Instantiate(_counterChangerPrefab, _counterChangersContainer);
            newCounterChanger.Removed += DeleteStatCounter;
            newCounterChanger.Value = newCounter;

            _statCounterChangers.Add(newCounterChanger);

            var addedSize = ((RectTransform) newCounterChanger.transform).sizeDelta.y;
            newCounterChanger.SizeChanged += SizeChanged;

            if (_statCounterChangers.Count >= 2)
                addedSize += _statCounterSpacing;

            SizeChanged?.Invoke(addedSize);

            if (addToValue)
                Value.StatCounters.Add(newCounter);
        }

        private void DeleteStatCounter(StatCounterChanger changer)
        {
            if (!_statCounterChangers.Remove(changer)) return;

            var reducedSize = ((RectTransform) changer.transform).sizeDelta.y;
            if (_statCounterChangers.Count > 0)
                reducedSize += _statCounterSpacing;

            SizeChanged?.Invoke(-reducedSize);

            Destroy(changer.gameObject);
            changer.Removed -= DeleteStatCounter;
            Value.StatCounters.Remove(changer.Value);
            _statCounterChangers.Remove(changer);
        }

        private void OnStatCounterAdded()
        {
            AddStatCounter(new StatCounterRecord(), true);
            if (_isMinimized)
                OnMinimize();
        }
    }
}