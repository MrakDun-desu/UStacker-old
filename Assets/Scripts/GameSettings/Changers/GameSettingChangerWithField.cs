using TMPro;
using UnityEngine;

namespace UStacker.GameSettings.Changers
{
    public abstract class GameSettingChangerWithField<T> : GameSettingChangerBase<T>
    {
        [SerializeField] protected TMP_InputField _valueField;

        protected void Start()
        {
            RefreshValue();
            _gameSettingsSO.SettingsReloaded += RefreshValue;
            _valueField.onEndEdit.AddListener(OnValueOverwritten);
        }

        protected virtual void RefreshValue()
        {
            _valueField.SetTextWithoutNotify(_gameSettingsSO.GetValue<T>(_controlPath).ToString());
        }

        protected abstract void OnValueOverwritten(string newValue);
    }
}