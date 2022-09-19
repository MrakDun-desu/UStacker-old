using System;
using System.Collections.Generic;
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
        private const string TITLE_CONTAINER_CLASS = "title-container";
        private const string BACK_BUTTON_CLASS = "back-btn";

        private const string ADD_GROUP_BUTTON_TEXT = "Add new group";
        private const string TITLE_TEXT = "Stat counting";
        private const string BACK_BUTTON_TEXT = "Back";
        public const string BACK_BUTTON_NAME = "Back";

        private readonly VisualElement _groupsContainer = new();

        private Dictionary<Guid, StatCounterGroup> _statCounterGroups;
        private readonly Dictionary<Guid, StatCounterGroupChanger> _groupChangers = new();
        private readonly PremadeCounterType[] _premadeCounterTypes;

        public StatCountingChanger() : this(Array.Empty<PremadeCounterType>())
        {
        }

        public StatCountingChanger(PremadeCounterType[] premadeCounterTypes)
        {
            _premadeCounterTypes = premadeCounterTypes;

            var titleContainer = new VisualElement();
            var title = new Label(TITLE_TEXT);
            titleContainer.AddToClassList(TITLE_CONTAINER_CLASS);
            titleContainer.Add(title);
            var backButton = new Button
            {
                text = BACK_BUTTON_TEXT,
                name = BACK_BUTTON_NAME
            };
            backButton.AddToClassList(BACK_BUTTON_CLASS);
            titleContainer.Add(backButton);

            var buttonContainer = new VisualElement();
            buttonContainer.AddToClassList(BUTTON_CONTAINER_CLASS);
            buttonContainer.Add(new Button(() => AddGroup(Guid.NewGuid(), new StatCounterGroup())){text = ADD_GROUP_BUTTON_TEXT});

            AddToClassList(SELF_CLASS);
            Add(titleContainer);
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

        private void AddGroup(Guid newId, StatCounterGroup newGroup)
        {
            var newGroupChanger = new StatCounterGroupChanger(_premadeCounterTypes)
            {
                Value = newGroup,
                Id = newId
            };

            _groupsContainer.Add(newGroupChanger);
            _statCounterGroups.Add(newId, newGroup);
            _groupChangers.Add(newId, newGroupChanger);
            newGroupChanger.GroupRemoved += OnGroupRemoved;
        }

        private void OnGroupRemoved(Guid groupId)
        {
            if (!_groupChangers.ContainsKey(groupId)) return;

            _groupsContainer.Remove(_groupChangers[groupId]);
            _groupChangers.Remove(groupId);
            _statCounterGroups.Remove(groupId);
        }
    }
}