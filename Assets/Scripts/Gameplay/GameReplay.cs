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
        public List<InputActionMessage> ActionList;
        public GameSettingsSO.SettingsContainer GameSettings;
        public StatContainer Stats;
        public double GameLength;
    }
}