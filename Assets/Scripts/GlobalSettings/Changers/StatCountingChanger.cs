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

        private Dictionary<Guid, StatCounterGroup> _statCounterGroups;
        private readonly Dictionary<Guid, StatCounterGroupChanger> _groupChangers = new();
        private RectTransform _parentTransform;
        private RectTransform _selfTransform;

        private void Awake()
        {
            _addGroupButton.onClick.AddListener(OnGroupAdded);
            _parentTransform = transform.parent.GetComponent<RectTransform>();

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
            _groupsContainer.position = new Vector2(
                _groupsContainer.position.x,
                _groupsContainer.sizeDelta.y / 2f
            );

            _selfTransform.sizeDelta = new Vector2(
                _selfTransform.sizeDelta.x,
                _selfTransform.sizeDelta.y + sizeDelta
            );

            _parentTransform.sizeDelta = new Vector2(
                _parentTransform.sizeDelta.x,
                _parentTransform.sizeDelta.y + sizeDelta
            );
        }

        private void AddGroup(Guid newId, StatCounterGroup newGroup, bool addToValue = false)
        {
            var newGroupChanger = Instantiate(_groupChangerPrefab, _groupsContainer);
            newGroupChanger.Value = newGroup;
            newGroupChanger.Id = newId;

            _groupChangers.Add(newId, newGroupChanger);

            newGroupChanger.GroupRemoved += OnGroupRemoved;
            newGroupChanger.SizeChanged += OnSizeChanged;

            if (addToValue)
                _statCounterGroups.Add(newId, newGroup);

        }

        private void OnGroupRemoved(Guid groupId)
        {
            if (!_groupChangers.ContainsKey(groupId)) return;

            foreach (var (key, _) in AppSettings.StatCounting.GameStatCounterDictionary.Where(pair => pair.Value == groupId))
            {
                AppSettings.StatCounting.GameStatCounterDictionary.Remove(key);
            }

            _groupChangers[groupId].GroupRemoved -= OnGroupRemoved;
            _groupChangers[groupId].SizeChanged -= OnSizeChanged;
            Destroy(_groupChangers[groupId].gameObject);

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