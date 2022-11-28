using Blockstacker.Common;

namespace Blockstacker.GameSettings.Changers.Files
{
    public class CustomGameManagerChanger : GameSettingFileChanger
    {
        protected override string TargetDir => PersistentPaths.GameManagers;
    }
}