
/************************************
GameSettingUIntChanger.cs -- created by Marek Dančo (xdanco00)
*************************************/
namespace UStacker.GameSettings.Changers
{
    public class GameSettingUintChanger : GameSettingChangerWithField<uint>
    {
        protected override void OnValueOverwritten(string value)
        {
            if (!uint.TryParse(value, out var uintValue))
            {
                RefreshValue();
                return;
            }

            SetValue(uintValue);
        }
    }
}
/************************************
end GameSettingUIntChanger.cs
*************************************/
