using System;
using Blockstacker.GameSettings.Enums;

namespace Blockstacker.GameSettings.SettingGroups
{
    [Serializable]
    public class ObjectiveSettings
    {
        public MainStat MainStat = MainStat.Time;
        public GameEndCondition GameEndCondition = GameEndCondition.LinesCleared;
        public float EndConditionCount = 40;
        public bool ToppingOutIsOkay = false;
        public bool GenerateCheese = false;
        public CheeseGeneration CheeseGeneration = CheeseGeneration.Singles;
        public int MaxCheeseHeight = 10;
        public bool UseCustomCheeseScript = false;
        public string CustomCheeseScriptName = "";
    }
}