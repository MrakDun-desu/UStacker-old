using System;
using System.Collections.Generic;
using System.Linq;
using Blockstacker.GlobalSettings.StatCounting;
using UnityEngine;
using UnityEngine.UI;

namespace Blockstacker.GlobalSettings.Changers
{
    public class StatCountingChanger : MonoBehaviour
    {
        [SerializeField] private RectTransform _groupsContainer;
        [SerializeField] private Button _addGroupButton;
        [SerializeField] private StatCounterGroupChanger _groupChangerPrefab;
        [SerializeField] private RectTransform _layoutRoot;
        [SerializeField] private float _groupChangerSpacing = 10;

        private Dictionary<Guid, StatCounterGroup> _statCounterGroups;
        private readonly Dictionary<Guid, StatCounterGroupChanger> _groupChangers = new();
        private RectTransform _parentTransform;
        private RectTransform _selfTransform;

        private void Awake()
        {
            var myTransform = transform;
            _parentTransform = myTransform.parent as RectTransform;
            _selfTransform = myTransform as RectTransform;
        }

        private void Start()
        {
            _addGroupButton.onClick.AddListener(OnGroupAdded);
            RefreshGroups();
            AppSettings.SettingsReloaded += RefreshGroups;
        }

        private void RefreshGroups()
        {
            foreach (var changer in _groupChangers.Values)
                Destroy(changer.gameObject);

            _groupChangers.Clear();

            _statCounterGroups = AppSettings.StatCounting.StatCounterGroups;

            foreach (var (id, group) in _statCounterGroups)
                AddGroup(id, group);
        }

        private void OnSizeChanged(float sizeDelta) {
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

            var groupsContainerPos = _groupsContainer.anchoredPosition;
            groupsContainerPos = new Vector2(
                groupsContainerPos.x,
                groupsContainerPos.y - sizeDelta / 2f
            );
            _groupsContainer.anchoredPosition = groupsContainerPos;
            LayoutRebuilder.MarkLayoutForRebuild(_layoutRoot);
            LayoutRebuilder.MarkLayoutForRebuild(_groupsContainer);
        }


        private void AddGroup(Guid newId, StatCounterGroup newGroup, bool addToValue = false)
        {
            var newGroupChanger = Instantiate(_groupChangerPrefab, _groupsContainer);
            newGroupChanger.Value = newGroup;
            newGroupChanger.Id = newId;

            _groupChangers.Add(newId, newGroupChanger);

            newGroupChanger.GroupRemoved += OnGroupRemoved;
            newGroupChanger.SizeChanged += OnSizeChanged;

            var sizeDelta = ((RectTransform) newGroupChanger.transform).sizeDelta.y;
            if (_groupChangers.Count >= 2)
                sizeDelta += _groupChangerSpacing;
            
            OnSizeChanged(sizeDelta);

            if (addToValue)
                _statCounterGroups.Add(newId, newGroup);
        }

        private void OnGroupRemoved(Guid groupId)
        {
            if (!_groupChangers.ContainsKey(groupId)) return;

            var keysToRemove = AppSettings.StatCounting.GameStatCounterDictionary.Where(pair => pair.Value == groupId).Select(pair => pair.Key).ToArray();
            // ReSharper disable once ForCanBeConvertedToForeach
            for (var i = 0; i < keysToRemove.Length; i++)
            {
                var key = keysToRemove[i];
                AppSettings.StatCounting.GameStatCounterDictionary.Remove(key);
            }

            var removedChanger = _groupChangers[groupId];
            
            removedChanger.GroupRemoved -= OnGroupRemoved;
            removedChanger.SizeChanged -= OnSizeChanged;

            var sizeDelta = ((RectTransform) removedChanger.transform).sizeDelta.y;

            if (_groupChangers.Count > 0)
                sizeDelta += _groupChangerSpacing;
            
            Destroy(removedChanger.gameObject);
            
            OnSizeChanged(-sizeDelta);

            _groupChangers.Remove(groupId);
            _statCounterGroups.Remove(groupId);
        }

        private void OnGroupAdded()
        {
            Guid newGuid;
            do
            {
                newGuid = Guid.NewGuid();
            } while (_statCounterGroups.ContainsKey(newGuid));
            AddGroup(newGuid, new StatCounterGroup(), true);
        }

    }
}