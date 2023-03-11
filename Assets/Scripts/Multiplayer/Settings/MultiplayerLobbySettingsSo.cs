using System;
using UnityEngine;

namespace UStacker.Multiplayer.Settings
{
    [CreateAssetMenu(fileName = "LobbySettings", menuName = "UStacker/Lobby settings asset")]
    public class MultiplayerLobbySettingsSo : ScriptableObject
    {
        public SettingsContainer Settings = new();

        [Serializable]
        public class SettingsContainer
        {
            [SerializeField] [Range(2, 2000)] private uint _playerLimit = 2;
            [SerializeField] [Range(1, 1000)] private uint _firstTo = 1;
            [SerializeField] [Range(1, 1000)] private uint _winBy = 1;

            public uint PlayerLimit
            {
                get => _playerLimit;
                // Max player limit is the same as the maximum clients on transport
                set => _playerLimit = Math.Clamp(value, 2, 2000);
            }

            public uint FirstTo
            {
                get => _firstTo;
                set => _firstTo = Math.Clamp(value, 1, 1000);
            }

            public uint WinBy
            {
                get => _winBy;
                set => _winBy = Math.Clamp(value, 1, 1000);
            }
        }
    }
}