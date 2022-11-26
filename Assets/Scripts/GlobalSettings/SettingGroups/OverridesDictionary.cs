using System;
using Blockstacker.Common;

namespace Blockstacker.GlobalSettings.Groups
{
    [Serializable]
    public class OverridesDictionary : SerializableDictionary<string, GameSettingsOverrides>
    {
    }
}