using Blockstacker.Settings.Changers;
using UnityEngine;

namespace Blockstacker.Settings.Appliers
{
    public abstract class SettingApplierBase : MonoBehaviour
    {
        protected ISettingChanger _settingChanger;

        protected void Awake() => _settingChanger = GetComponent<ISettingChanger>();

        protected void OnEnable() => _settingChanger.SettingChanged += OnSettingChanged;

        protected void OnDisable() => _settingChanger.SettingChanged -= OnSettingChanged;

        public abstract void OnSettingChanged();
    }
}