using System;
using System.IO;
using Blockstacker.Common;
using Blockstacker.Common.Alerts;
using Blockstacker.Common.Attributes;
using Blockstacker.GameSettings.Enums;
using UnityEngine;

namespace Blockstacker.GameSettings.SettingGroups
{
    [Serializable]
    public record ObjectiveSettings
    {
        // backing fields

        private GameManagerType _gameManagerType = GameManagerType.None;
        private string _customGameManager = string.Empty;
        private GarbageGeneration _garbageGeneration;
        private string _customGarbageGeneratorName = string.Empty;
        private double _endConditionCount = 40d;
        private uint _garbageHeight = 10;

        public MainStat MainStat { get; set; } = MainStat.Time;

        public GameEndCondition GameEndCondition { get; set; } = GameEndCondition.LinesCleared;

        public double EndConditionCount
        {
            get => _endConditionCount;
            set => _endConditionCount = Math.Max(value, 0);
        }

        public bool ToppingOutIsOkay { get; set; }

        public string StartingLevel { get; set; }

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

        // not shown in settings UI
        public string CustomGarbageScript { get; set; } = "";
        public string CustomGameManagerScript { get; set; } = "";

        private bool TryReloadGarbageGenerator()
        {
            const string filenameExtension = ".lua";
            var filePath = Path.Combine(CustomizationPaths.RotationSystems,
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

            if (TryReloadGarbageGenerator())
            {
                _ = AlertDisplayer.Instance.ShowAlert(
                    new Alert("Custom garbage script load failed!",
                        $"Garbage script {CustomGarbageScriptName} couldn't be found.",
                        AlertType.Error));
            }
            else
            {
                _ = AlertDisplayer.Instance.ShowAlert(
                    new Alert("Custom garbage script loaded!",
                        $"Garbage script {CustomGarbageScriptName} was loaded into game settings.",
                        AlertType.Success));
            }
        }

        private bool TryReloadGameManagerScript()
        {
            const string filenameExtension = ".lua";
            var filePath = Path.Combine(CustomizationPaths.RotationSystems,
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

            if (TryReloadGameManagerScript())
            {
                _ = AlertDisplayer.Instance.ShowAlert(
                    new Alert("Custom game manager load failed!",
                        $"Game manager {CustomGameManager} couldn't be found.",
                        AlertType.Error));
            }
            else
            {
                _ = AlertDisplayer.Instance.ShowAlert(
                    new Alert("Custom game manager loaded!",
                        $"Game manager {CustomGameManager} was loaded into game settings.",
                        AlertType.Success));
            }
        }
    }
}