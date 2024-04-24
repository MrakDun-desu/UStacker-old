
/************************************
GameSettingChangerWithField.cs -- created by Marek Dančo (xdanco00)
*************************************/
using TMPro;
using UnityEngine;

namespace UStacker.GameSettings.Changers
{
    public abstract class GameSettingChangerWithField<T> : GameSettingChangerBase<T>
    {
        [SerializeField] protected TMP_InputField _valueField;

        protected override void Start()
        {
            base.Start();
            _valueField.onEndEdit.AddListener(OnValueOverwritten);
        }

        protected override void RefreshValue()
        {
            _valueField.SetTextWithoutNotify(_gameSettingsSO.GetValue<T>(_controlPath).ToString());
        }

        protected abstract void OnValueOverwritten(string newValue);
    }
}
/************************************
end GameSettingChangerWithField.cs
*************************************/
