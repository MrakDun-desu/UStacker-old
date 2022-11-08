using Blockstacker.Common;

namespace Blockstacker.GameSettings.Changers.Files
{
    public class CustomRotationSystemChanger : GameSettingFileChanger
    {
        protected override string TargetDir => CustomizationPaths.RotationSystems;
    }
}