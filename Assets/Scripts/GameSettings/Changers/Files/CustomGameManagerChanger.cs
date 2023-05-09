
/************************************
CustomGameManagerChanger.cs -- created by Marek Dančo (xdanco00)
*************************************/
using UStacker.Common;

namespace UStacker.GameSettings.Changers.Files
{
    public class CustomGameManagerChanger : GameSettingFileChanger
    {
        protected override string TargetDir => PersistentPaths.GameManagers;
    }
}
/************************************
end CustomGameManagerChanger.cs
*************************************/
