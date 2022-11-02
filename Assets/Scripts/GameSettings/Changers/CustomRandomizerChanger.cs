using Blockstacker.Common;

namespace Blockstacker.GameSettings.Changers
{
    public class CustomRandomizerChanger : GameSettingFileChanger
    {
        protected override string DefaultEmptyPrompt => "No randomizer available";
        protected override string TargetDir => CustomizationPaths.Randomizers;
    }
}