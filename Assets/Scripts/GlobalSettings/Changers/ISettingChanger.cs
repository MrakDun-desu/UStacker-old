using System;

namespace Blockstacker.GlobalSettings.Changers
{
    public interface ISettingChanger
    {
        event Action SettingChanged;
    }
}