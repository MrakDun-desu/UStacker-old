
/************************************
GameSettingDoubleChanger.cs -- created by Marek Dančo (xdanco00)
*************************************/
using System.Globalization;
using UStacker.Common.Extensions;

namespace UStacker.GameSettings.Changers
{
    public class GameSettingDoubleChanger : GameSettingChangerWithField<double>
    {
        protected override void RefreshValue()
        {
            _valueField.SetTextWithoutNotify(_gameSettingsSO.GetValue<double>(_controlPath)
                .ToString(CultureInfo.InvariantCulture));
        }

        protected override void OnValueOverwritten(string newValue)
        {
            if (!newValue.TryParseDouble(out var doubleValue))
            {
                RefreshValue();
                return;
            }

            SetValue(doubleValue);
            var actualValue = _gameSettingsSO.GetValue<double>(_controlPath);
            _valueField.SetTextWithoutNotify(actualValue.ToString(CultureInfo.InvariantCulture));
        }
    }
}
/************************************
end GameSettingDoubleChanger.cs
*************************************/
