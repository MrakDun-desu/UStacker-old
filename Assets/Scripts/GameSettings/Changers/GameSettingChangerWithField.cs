using TMPro;
using UnityEngine;

namespace Blockstacker.GameSettings.Changers
{
    public class GameSettingChangerWithField<T> : GameSettingChangerBase<T>
    {
        [SerializeField] protected TMP_InputField _valueField;

        protected void Start()
        {
            _valueField.text = _gameSettingsSO.GetValue<T>(_controlPath).ToString();
        }
    }
}