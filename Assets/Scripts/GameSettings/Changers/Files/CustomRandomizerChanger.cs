using Blockstacker.Common;

namespace Blockstacker.GameSettings.Changers.Files
{
    public class CustomRandomizerChanger : GameSettingFileChanger
    {
        protected override string TargetDir => PersistentPaths.Randomizers;
    }
}