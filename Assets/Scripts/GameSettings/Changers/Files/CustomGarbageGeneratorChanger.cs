using UStacker.Common;
using UStacker.GlobalSettings;

namespace UStacker.GameSettings.Changers.Files
{
    public class CustomGarbageGeneratorChanger : GameSettingFileChanger
    {
        protected override string TargetDir => PersistentPaths.GarbageGenerators;
    }
}