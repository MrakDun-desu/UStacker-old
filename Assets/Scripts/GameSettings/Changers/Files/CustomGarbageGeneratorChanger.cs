
/************************************
CustomGarbageGeneratorChanger.cs -- created by Marek Dančo (xdanco00)
*************************************/
using UStacker.Common;

namespace UStacker.GameSettings.Changers.Files
{
    public class CustomGarbageGeneratorChanger : GameSettingFileChanger
    {
        protected override string TargetDir => PersistentPaths.GarbageGenerators;
    }
}
/************************************
end CustomGarbageGeneratorChanger.cs
*************************************/
