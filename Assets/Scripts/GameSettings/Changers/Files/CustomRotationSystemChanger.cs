
/************************************
CustomRotationSystemChanger.cs -- created by Marek Dančo (xdanco00)
*************************************/
using UStacker.Common;

namespace UStacker.GameSettings.Changers.Files
{
    public class CustomRotationSystemChanger : GameSettingFileChanger
    {
        protected override string TargetDir => PersistentPaths.RotationSystems;
    }
}
/************************************
end CustomRotationSystemChanger.cs
*************************************/
