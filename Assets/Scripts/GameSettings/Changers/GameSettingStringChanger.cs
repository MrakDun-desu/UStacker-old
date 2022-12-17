namespace UStacker.GameSettings.Changers
{
    public class GameSettingStringChanger : GameSettingChangerWithField<string>
    {
        protected override void OnValueOverwritten(string newValue)
        {
            SetValue(newValue);
            var actualValue = _gameSettingsSO.GetValue<string>(_controlPath);
            _valueField.SetTextWithoutNotify(actualValue);
        }
    }
}