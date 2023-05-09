
/************************************
GameSettingFloatChanger.cs -- created by Marek Dančo (xdanco00)
*************************************/
using System.Globalization;

namespace UStacker.GameSettings.Changers
{
    public class GameSettingFloatChanger : GameSettingChangerWithField<float>
    {
        protected override void RefreshValue()
        {
            _valueField.SetTextWithoutNotify(_gameSettingsSO.GetValue<float>(_controlPath)
                .ToString(CultureInfo.InvariantCulture));
        }

        protected override void OnValueOverwritten(string value)
        {
            value = value.Replace('.', ',');
            if (!float.TryParse(value, out var floatValue))
            {
                RefreshValue();
                return;
            }

            SetValue(floatValue);
        }
    }
}
/************************************
end GameSettingFloatChanger.cs
*************************************/
