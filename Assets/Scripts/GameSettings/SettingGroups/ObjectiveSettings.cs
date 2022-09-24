using System;
using Blockstacker.GameSettings.Enums;

namespace Blockstacker.GameSettings.SettingGroups
{
    [Serializable]
    public record ObjectiveSettings
    {
        public MainStat MainStat = MainStat.Time;
        public GameEndCondition GameEndCondition = GameEndCondition.LinesCleared;
        public double EndConditionCount = 40;
        public bool ToppingOutIsOkay;
        public GarbageGeneration GarbageGeneration = GarbageGeneration.None;
        public uint GarbageHeight = 10;
        public uint StartingLevel;
        public GameManagerType GameManagerType = GameManagerType.None;
        public string CustomGameManagerName = "";
        public string CustomGameManagerScript = "";
        public bool UseCustomGarbageScript;
        public string CustomGarbageScriptName = "";
        public string CustomGarbageScript = "";
    }
}