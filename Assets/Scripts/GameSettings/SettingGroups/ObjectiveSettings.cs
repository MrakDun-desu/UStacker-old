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
        public bool GenerateCheese;
        public CheeseGeneration CheeseGeneration = CheeseGeneration.Singles;
        public int MaxCheeseHeight = 10;
        public bool UseCustomCheeseScript;
        public string CustomCheeseScriptName = "";
        public string CustomCheeseScript = "";
    }
}