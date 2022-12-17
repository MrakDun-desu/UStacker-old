using System;
using UStacker.Common;

namespace UStacker.GlobalSettings.Groups
{
    [Serializable]
    public class OverridesDictionary : SerializableDictionary<string, GameSettingsOverrides>
    {
    }
}