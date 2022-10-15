using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;

namespace Blockstacker.GlobalSettings.StatCounting.UI
{
    public class StatCountingChanger : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<StatCountingChanger>
        {
        }

        private const string SELF_CLASS = "stat-counting-changer";
        private const string BUTTON_CONTAINER_CLASS = "button-container";

        private const string ADD_GROUP_BUTTON_TEXT = "Add new group";
        public const string BACK_BUTTON_NAME = "Back";

        private readonly VisualElement _groupsContainer = new();

        private Dictionary<Guid, StatCounterGroup> _statCounterGroups;
        private readonly Dictionary<Guid, StatCounterGroupChanger> _groupChangers = new();
        private readonly StatCounterSO[] _premadeCounterTypes;

        public StatCountingChanger() : this(Array.Empty<StatCounterSO>())
        {
        }

        public StatCountingChanger(StatCounterSO[] premadeCounterTypes)
        {
            _premadeCounterTypes = premadeCounterTypes;

            var buttonContainer = new VisualElement();
            buttonContainer.AddToClassList(BUTTON_CONTAINER_CLASS);
            buttonContainer.Add(new Button(OnGroupAdded){text = ADD_GROUP_BUTTON_TEXT});

            AddToClassList(SELF_CLASS);
            Add(_groupsContainer);
            Add(buttonContainer);

            RefreshGroups();
            AppSettings.SettingsReloaded += RefreshGroups;
        }

        private void RefreshGroups()
        {
            foreach (var changer in _groupChangers.Values)
                _groupsContainer.Remove(changer);

            _groupChangers.Clear();
            _statCounterGroups = AppSettings.StatCounting.StatCounterGroups;

            foreach (var (id, group) in _statCounterGroups)
                AddGroup(id, group);
        }

        private void AddGroup(Guid newId, StatCounterGroup newGroup, bool addToValue = false)
        {
            var newGroupChanger = new StatCounterGroupChanger(_premadeCounterTypes)
            {
                Value = newGroup,
                Id = newId
            };

            _groupsContainer.Add(newGroupChanger);
            _groupChangers.Add(newId, newGroupChanger);
            newGroupChanger.GroupRemoved += OnGroupRemoved;
            if (addToValue)
                _statCounterGroups.Add(newId, newGroup);
        }

        private void OnGroupRemoved(Guid groupId)
        {
            if (!_groupChangers.ContainsKey(groupId)) return;

            foreach (var (key, _) in AppSettings.StatCounting.GameStatCounterDictionary.Where(pair => pair.Value == groupId))
                AppSettings.StatCounting.GameStatCounterDictionary.Remove(key);
            
            _groupsContainer.Remove(_groupChangers[groupId]);
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
            AddGroup(Guid.NewGuid(), new StatCounterGroup(), true);
        }
    }
}