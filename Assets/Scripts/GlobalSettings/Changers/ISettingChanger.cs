using System;

namespace UStacker.GlobalSettings.Changers
{
    public interface ISettingChanger
    {
        event Action SettingChanged;
    }
}