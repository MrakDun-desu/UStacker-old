using System.Collections.Generic;
using UStacker.GlobalSettings.StatCounting;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UStacker.GlobalSettings.Changers
{
    public class DefaultCounterGroupChanger : MonoBehaviour
    {
        [SerializeField] private TMP_Text _nameLabel;
        [SerializeField] private Button _minimizeButton;
        [SerializeField] private TMP_Text _minimizeButtonText;
        [SerializeField] private Button _counterAddButton;
        [SerializeField] private RectTransform _counterChangersContainer;
        [SerializeField] private ContentSizeFitter _containerSizeFitter;
        [SerializeField] private StatCounterChanger _counterChangerPrefab;
        [SerializeField] private RectTransform _layoutRoot;
        [SerializeField] private float _statCounterSpacing = 10;

        private static StatCounterGroup Value => AppSettings.StatCounting.DefaultGroup;
        private readonly List<StatCounterChanger> _statCounterChangers = new();
        private float _formerHeight;
        private bool _isMinimized;
        private RectTransform _selfTransform;
        private RectTransform _parentTransform;
        
        private void Awake()
        {
            var myTransform = transform;
            _selfTransform = myTransform as RectTransform;
            _parentTransform = myTransform.parent as RectTransform;
            _nameLabel.text = AppSettings.StatCounting.DefaultGroup.Name;
            AppSettings.SettingsReloaded += RefreshStatCounters;
        }

        private void Start()
        {
            _minimizeButton.onClick.AddListener(OnMinimize);
            _counterAddButton.onClick.AddListener(OnStatCounterAdded);
            RefreshStatCounters();
        }

        private void ChangeHeight(float sizeDelta)
        {
            var selfSizeDelta = _selfTransform.sizeDelta;
            selfSizeDelta = new Vector2(
                selfSizeDelta.x,
                selfSizeDelta.y + sizeDelta
            );
            _selfTransform.sizeDelta = selfSizeDelta;
            
            var parentSizeDelta = _parentTransform.sizeDelta;
            parentSizeDelta = new Vector2(
                parentSizeDelta.x,
                parentSizeDelta.y + sizeDelta
            );
            _parentTransform.sizeDelta = parentSizeDelta;

            var counterChangersPos = _counterChangersContainer.anchoredPosition;
            counterChangersPos = new Vector2(
                counterChangersPos.x,
                counterChangersPos.y - sizeDelta / 2f
            );
            _counterChangersContainer.anchoredPosition = counterChangersPos;
            
            LayoutRebuilder.MarkLayoutForRebuild(_layoutRoot);
        }

        private void RefreshStatCounters()
        {
            foreach (var counterChanger in _statCounterChangers)
            {
                counterChanger.Removed -= DeleteStatCounter;
                DeleteStatCounter(counterChanger);
            }

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
            ChangeHeight(_formerHeight * (_isMinimized ? -1 : 1));
        }

        private void AddStatCounter(StatCounterRecord newCounter, bool addToValue = false)
        {
            var newCounterChanger = Instantiate(_counterChangerPrefab, _counterChangersContainer);
            newCounterChanger.Removed += DeleteStatCounter;
            newCounterChanger.Value = newCounter;

            _statCounterChangers.Add(newCounterChanger);

            var addedSize = ((RectTransform) newCounterChanger.transform).sizeDelta.y;
            newCounterChanger.SizeChanged += ChangeHeight;

            if (_statCounterChangers.Count >= 2)
                addedSize += _statCounterSpacing;

            ChangeHeight(addedSize);

            if (addToValue)
                Value.StatCounters.Add(newCounter);
        }

        private void DeleteStatCounter(StatCounterChanger changer)
        {
            if (!_statCounterChangers.Remove(changer)) return;

            var reducedSize = ((RectTransform) changer.transform).sizeDelta.y;
            if (_statCounterChangers.Count > 0)
                reducedSize += _statCounterSpacing;

            ChangeHeight(-reducedSize);

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