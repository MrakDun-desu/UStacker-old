using System;
using Blockstacker.GlobalSettings.StatCounting;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Blockstacker.GlobalSettings.Changers
{
    public class StatCounterGroupChanger : MonoBehaviour
    {
        [SerializeField] private TMP_InputField _nameField;
        [SerializeField] private Button _minimizeButton;
        [SerializeField] private RectTransform _counterChangersContainer;

        public StatCounterGroup Value
        {
            get => _value;
            set
            {
                _value = value;
                _nameField.SetTextWithoutNotify(value.Name);
                // RefreshStatCounters();
            }
        }

        public Guid Id { get; set; }
        public event Action<Guid> GroupRemoved;
            
        private StatCounterGroup _value;

        private void Awake()
        {
            _nameField.SetTextWithoutNotify("Group name");
            _minimizeButton.onClick.AddListener(OnMinimize);
        }

        private void OnMinimize()
        {
            
        }
    }
}