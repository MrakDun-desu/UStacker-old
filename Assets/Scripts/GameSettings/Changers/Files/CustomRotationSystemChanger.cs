using UStacker.Common;
using UStacker.GlobalSettings;

namespace UStacker.GameSettings.Changers.Files
{
    public class CustomRotationSystemChanger : GameSettingFileChanger
    {
        protected override string TargetDir => PersistentPaths.RotationSystems;
    }
}