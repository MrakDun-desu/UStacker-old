
/************************************
GameSettingIntChanger.cs -- created by Marek Dančo (xdanco00)
*************************************/
namespace UStacker.GameSettings.Changers
{
    public class GameSettingIntChanger : GameSettingChangerWithField<int>
    {
        protected override void OnValueOverwritten(string value)
        {
            if (!int.TryParse(value, out var intValue))
            {
                RefreshValue();
                return;
            }

            SetValue(intValue);
        }
    }
}
/************************************
end GameSettingIntChanger.cs
*************************************/
