using Blockstacker.Common;

namespace Blockstacker.GameSettings.Changers
{
    public class CustomGameManagerChanger : GameSettingFileChanger
    {
        protected override string TargetDir => CustomizationPaths.GameManagers;
        protected override string DefaultEmptyPrompt => "No game manager available";
    }
}