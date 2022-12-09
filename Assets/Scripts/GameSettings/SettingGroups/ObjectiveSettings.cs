using System;
using System.IO;
using Blockstacker.Common;
using Blockstacker.Common.Alerts;
using Blockstacker.GameSettings.Enums;
using UnityEngine;

namespace Blockstacker.GameSettings.SettingGroups
{
    [Serializable]
    public record ObjectiveSettings
    {
        // backing fields
        [SerializeField]
        private GameManagerType _gameManagerType = GameManagerType.None;
        [SerializeField]
        private string _customGameManager = string.Empty;
        [SerializeField]
        private GarbageGeneration _garbageGeneration;
        [SerializeField]
        private string _customGarbageGeneratorName = string.Empty;
        [SerializeField]
        private double _endConditionCount = 40d;
        [SerializeField]
        private uint _garbageHeight = 10;

        [field: SerializeField]
        public MainStat MainStat { get; set; } = MainStat.Time;
        [field: SerializeField]
        public GameEndCondition GameEndCondition { get; set; } = GameEndCondition.LinesCleared;
        [field: SerializeField]
        public string CustomGarbageScript { get; set; } = "";
        [field: SerializeField]
        public string CustomGameManagerScript { get; set; } = "";
        [field: SerializeField]
        public bool ToppingOutIsOkay { get; set; }

        [field: SerializeField] public string StartingLevel { get; set; } = string.Empty;

        public double EndConditionCount
        {
            get => _endConditionCount;
            set => _endConditionCount = Math.Max(value, 0);
        }

        public GameManagerType GameManagerType
        {
            get => _gameManagerType;
            set
            {
                _gameManagerType = value;
                ReloadGameManagerIfNeeded();
            }
        }

        public string CustomGameManager
        {
            get => _customGameManager;
            set
            {
                _customGameManager = value;
                ReloadGameManagerIfNeeded();
            }
        }

        public GarbageGeneration GarbageGeneration
        {
            get => _garbageGeneration;
            set
            {
                _garbageGeneration = value;
                ReloadGarbageGeneratorIfNeeded();
            }
        }

        public uint GarbageHeight
        {
            get => _garbageHeight;
            set => _garbageHeight = Math.Max(value, 400);
        }

        public string CustomGarbageScriptName
        {
            get => _customGarbageGeneratorName;
            set
            {
                _customGarbageGeneratorName = value;
                ReloadGarbageGeneratorIfNeeded();
            }
        }

        private bool TryReloadGarbageGenerator()
        {
            const string filenameExtension = ".lua";
            var filePath = Path.Combine(PersistentPaths.GarbageGenerators,
                CustomGarbageScriptName + filenameExtension);

            if (!File.Exists(filePath)) return false;

            CustomGarbageScript = File.ReadAllText(filePath);

            return true;
        }

        private void ReloadGarbageGeneratorIfNeeded()
        {
            if (!_garbageGeneration.HasFlag(GarbageGeneration.CustomFlag) ||
                string.IsNullOrEmpty(_customGarbageGeneratorName))
                return;

            var shownAlert = TryReloadGarbageGenerator()
                ? new Alert("Custom garbage script load failed!",
                    $"Garbage script {CustomGarbageScriptName} couldn't be found.",
                    AlertType.Error)
                : new Alert("Custom garbage script loaded!",
                    $"Garbage script {CustomGarbageScriptName} was loaded into game settings.",
                    AlertType.Success);

            _ = AlertDisplayer.Instance.ShowAlert(shownAlert);
        }

        private bool TryReloadGameManagerScript()
        {
            const string filenameExtension = ".lua";
            var filePath = Path.Combine(PersistentPaths.GameManagers,
                CustomGameManager + filenameExtension);

            if (!File.Exists(filePath)) return false;

            CustomGameManagerScript = File.ReadAllText(filePath);

            return true;
        }

        private void ReloadGameManagerIfNeeded()
        {
            if (_gameManagerType != GameManagerType.Custom ||
                string.IsNullOrEmpty(_customGameManager))
                return;

            var shownAlert = TryReloadGameManagerScript()
                ? new Alert("Custom game manager load failed!",
                    $"Game manager {CustomGameManager} couldn't be found.",
                    AlertType.Error)
                : new Alert("Custom game manager loaded!",
                    $"Game manager {CustomGameManager} was loaded into game settings.",
                    AlertType.Success);

            _ = AlertDisplayer.Instance.ShowAlert(shownAlert);
        }
    }
}