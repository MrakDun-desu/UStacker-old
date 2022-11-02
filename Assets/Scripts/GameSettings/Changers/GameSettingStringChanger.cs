namespace Blockstacker.GameSettings.Changers
{
    public class GameSettingStringChanger : GameSettingChangerWithField<string>
    {
        protected override void OnValueOverwritten(string newValue)
        {
            SetValue(newValue);
        }
    }
}