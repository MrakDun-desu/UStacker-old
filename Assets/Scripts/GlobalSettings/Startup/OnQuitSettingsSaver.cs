using Blockstacker.Common;

namespace Blockstacker.GlobalSettings.Startup
{
    public class OnQuitSettingsSaver : MonoSingleton<OnQuitSettingsSaver>
    {
        private void OnApplicationQuit()
        {
            AppSettings.TrySave();
        }
    }
}