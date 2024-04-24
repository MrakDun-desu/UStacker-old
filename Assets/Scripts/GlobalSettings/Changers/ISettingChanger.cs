
/************************************
ISettingChanger.cs -- created by Marek Dančo (xdanco00)
*************************************/
using System;

namespace UStacker.GlobalSettings.Changers
{
    public interface ISettingChanger
    {
        event Action SettingChanged;
    }
}
/************************************
end ISettingChanger.cs
*************************************/
