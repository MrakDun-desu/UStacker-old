using UStacker.Common;
using UStacker.GlobalSettings;

namespace UStacker.GameSettings.Changers.Files
{
    public class CustomGameManagerChanger : GameSettingFileChanger
    {
        protected override string TargetDir => PersistentPaths.GameManagers;
    }
}