
/************************************
OnQuitSettingsSaver.cs -- created by Marek Dančo (xdanco00)
*************************************/
using UStacker.Common;

namespace UStacker.GlobalSettings.Startup
{
    public class OnQuitSettingsSaver : MonoSingleton<OnQuitSettingsSaver>
    {
        private async void OnApplicationQuit()
        {
            await AppSettings.TrySaveAsync();
        }
    }
}
/************************************
end OnQuitSettingsSaver.cs
*************************************/
