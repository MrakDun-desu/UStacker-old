using System;

namespace Blockstacker.Settings.Changers
{
    public interface ISettingChanger
    {
        event Action SettingChanged;
    }
}