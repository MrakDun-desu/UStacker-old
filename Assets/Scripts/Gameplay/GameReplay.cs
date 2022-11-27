using System;
using System.Collections.Generic;
using Blockstacker.Gameplay.Communication;
using Blockstacker.Gameplay.Stats;
using Blockstacker.GameSettings;

namespace Blockstacker.Gameplay
{
    [Serializable]
    public record GameReplay
    {
        public List<InputActionMessage> ActionList { get; set; }
        public GameSettingsSO.SettingsContainer GameSettings { get; set; }
        public StatContainer Stats { get; set; }
        public double GameLength { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}