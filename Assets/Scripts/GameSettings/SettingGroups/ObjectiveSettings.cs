using System;
using Blockstacker.GameSettings.Enums;

namespace Blockstacker.GameSettings.SettingGroups
{
    [Serializable]
    public record ObjectiveSettings
    {
        public bool LowerScoreIsBetter = false;
        public MainStat MainStat = MainStat.Time;
        public GameEndCondition GameEndCondition = GameEndCondition.LinesCleared;
        public double EndConditionCount = 40;
        public bool ToppingOutIsOkay;
        public GarbageGeneration _garbageGeneration = GarbageGeneration.None;
        public uint GarbageHeight = 10;
        public bool UseCustomGarbageScript;
        public string CustomGarbageScriptName = "";
        public string CustomGarbageScript = "";
    }
}