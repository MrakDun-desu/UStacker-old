
/************************************
GameSettingStringChanger.cs -- created by Marek Dančo (xdanco00)
*************************************/
namespace UStacker.GameSettings.Changers
{
    public class GameSettingStringChanger : GameSettingChangerWithField<string>
    {
        protected override void OnValueOverwritten(string newValue)
        {
            SetValue(newValue);
        }
    }
}
/************************************
end GameSettingStringChanger.cs
*************************************/
