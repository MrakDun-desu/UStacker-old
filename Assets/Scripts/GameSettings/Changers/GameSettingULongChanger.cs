
/************************************
GameSettingULongChanger.cs -- created by Marek Dančo (xdanco00)
*************************************/
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
/************************************
end GameSettingULongChanger.cs
*************************************/
