namespace UStacker.GameSettings.Changers
{
    public class GameSettingULongChanger : GameSettingChangerWithField<ulong>
    {
        protected override void OnValueOverwritten(string value)
        {
            if (!ulong.TryParse(value, out var ulongValue))
            {
                RefreshValue();
                return;
            }

            SetValue(ulongValue);
        }
    }
}