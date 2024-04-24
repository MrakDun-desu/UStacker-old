
/************************************
CustomRandomizerChanger.cs -- created by Marek Dančo (xdanco00)
*************************************/
using UStacker.Common;

namespace UStacker.GameSettings.Changers.Files
{
    public class CustomRandomizerChanger : GameSettingFileChanger
    {
        protected override string TargetDir => PersistentPaths.Randomizers;
    }
}
/************************************
end CustomRandomizerChanger.cs
*************************************/
