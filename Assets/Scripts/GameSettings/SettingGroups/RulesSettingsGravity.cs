using System;
using Blockstacker.GameSettings.Enums;

namespace Blockstacker.GameSettings.SettingGroups
{
    [Serializable]
    public record RulesSettingsGravity
    {
        public double DefaultGravity = .02d;
        public double DefaultLockDelay = .5d;
        public LockDelayType LockDelayType = LockDelayType.OnTouchGround;
    }
}