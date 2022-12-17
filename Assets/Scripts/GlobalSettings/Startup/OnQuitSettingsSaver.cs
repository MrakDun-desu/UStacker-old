using UStacker.Common;

namespace UStacker.GlobalSettings.Startup
{
    public class OnQuitSettingsSaver : MonoSingleton<OnQuitSettingsSaver>
    {
        private void OnApplicationQuit()
        {
            AppSettings.TrySave();
        }
    }
}