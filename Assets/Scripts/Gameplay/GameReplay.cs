using System;
using System.Collections.Generic;
using Blockstacker.Gameplay;
using Blockstacker.Gameplay.Communication;
using Blockstacker.GameSettings;

namespace Blockstacker.Gameplay
{
    [Serializable]
    public class GameReplay
    {
        public List<InputActionMessage> ActionList;
        public GameSettingsSO.SettingsContainer GameSettings;
        public StatContainer Stats;
        public TimeSpan GameLength;
    }
}