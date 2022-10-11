using System;
using Blockstacker.Common.Attributes;
using Blockstacker.GameSettings.Enums;
using UnityEngine;

namespace Blockstacker.GameSettings.SettingGroups
{
    [Serializable]
    public record ObjectiveSettings
    {
        [Tooltip("Which stat is displayed as the main one")]
        public MainStat MainStat = MainStat.Time;
        
        [Tooltip("When to end the game")]
        public GameEndCondition GameEndCondition = GameEndCondition.LinesCleared;
        
        [MinRestraint(0, true)]
        public double EndConditionCount = 40;
        
        [Tooltip("If set, topping out will be counted as finishing the game, not as a loss")]
        public bool ToppingOutIsOkay;
        
        [Tooltip("On which level to start")]
        public string StartingLevel;
        
        [Tooltip("Controls levelling, score and can be programmed to completely alter the rules of the game")]
        public GameManagerType GameManagerType = GameManagerType.None;
        
        [Tooltip("Filename of the custom manager you want to use")]
        public string CustomGameManagerName = "";
        
        [Tooltip("How tall holes to generate (if set to none, no garbage will be generated)")]
        public GarbageGeneration GarbageGeneration = GarbageGeneration.None;
        
        [Tooltip("How high should the garbage be on board (may not work with custom generators)")]
        public uint GarbageHeight = 10;
        
        [Tooltip("If set, Garbage Generation is ignored and custom script is used instead")]
        public bool UseCustomGarbageScript;
        
        [Tooltip("Filename of the custom garbage script you want to use")]
        public string CustomGarbageScriptName = "";
        
        // not shown in settings UI
        public string CustomGarbageScript = "";
        public string CustomGameManagerScript = "";
    }
}