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