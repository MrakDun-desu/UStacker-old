using System;
using System.ComponentModel;
using UnityEngine;

namespace Blockstacker.GlobalSettings.Groups
{
    [Serializable]
    public record CustomizationSettings
    {
        [Tooltip("Folder of the current skin")]
        [Description("Skin")]
        public string SkinFolder = "";
        
        [Tooltip("Folder of the current sound pack")]
        [Description("Sound Pack")]
        public string SoundPackFolder = "";
        
        [Tooltip("Folder of the current background pack")]
        [Description("Background Pack")]
        public string BackgroundFolder = "";
    }
}