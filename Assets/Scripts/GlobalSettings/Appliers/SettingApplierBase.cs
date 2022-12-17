using Blockstacker.GlobalSettings.Changers;
using UnityEngine;

namespace Blockstacker.GlobalSettings.Appliers
{
    public abstract class SettingApplierBase : MonoBehaviour
    {
        private ISettingChanger _settingChanger;

        protected virtual void Awake()
        {
            _settingChanger = GetComponent<ISettingChanger>();
        }

        protected void OnEnable()
        {
            _settingChanger.SettingChanged += OnSettingChanged;
        }

        protected void OnDisable()
        {
            _settingChanger.SettingChanged -= OnSettingChanged;
        }

        public abstract void OnSettingChanged();
    }
}