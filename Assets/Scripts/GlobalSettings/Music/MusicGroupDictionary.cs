using System;
using System.Collections.Generic;
using UStacker.Common;

namespace UStacker.GlobalSettings.Music
{
    [Serializable]
    public class MusicGroupDictionary : SerializableDictionary<string, List<string>>
    {
    }
}