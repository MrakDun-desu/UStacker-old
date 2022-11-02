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
        private string _customGameManagerName = string.Empty;
        private GarbageGeneration _garbageGeneration;
        private string _customGarbageGeneratorName = string.Empty;
        
        [Tooltip("Which stat is displayed as the main one")]
        public MainStat MainStat = MainStat.Time;
        
        [Tooltip("When to end the game")]
        public GameEndCondition GameEndCondition = GameEndCondition.LinesCleared;
        
        [MinRestraint(0, true)]
        public double EndConditionCount = 40;
        
        [Tooltip("If set, topping out will be counted as finishing the game, not as a loss")]
        public bool ToppingOutIsOkay;
        
        [Tooltip("On which level to start")]
        public string StartingLevel;

        [Tooltip("Controls levelling, score and can be programmed to completely alter the rules of the game")]
        public GameManagerType GameManagerType
        {
            get => _gameManagerType;
            set
            {
                _gameManagerType = value;
                ReloadGameManagerIfNeeded();
            }
        }

        [Tooltip("Filename of the custom manager you want to use")]
        public string CustomGameManagerName
        {
            get => _customGameManagerName;
            set
            {
                _customGameManagerName = value;
                ReloadGameManagerIfNeeded();
            }
        }

        [Tooltip("How tall holes to generate (if set to none, no garbage will be generated)")]
        public GarbageGeneration GarbageGeneration
        {
            get => _garbageGeneration;
            set
            {
                _garbageGeneration = value;
                ReloadGarbageGeneratorIfNeeded();
            }
        }
        
        [Tooltip("How high should the garbage be on board (might not work with custom generators)")]
        public uint GarbageHeight = 10;

        [Tooltip("Filename of the custom garbage script you want to use")]
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
        public string CustomGarbageScript = "";
        public string CustomGameManagerScript = "";
        
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
            if (_garbageGeneration != GarbageGeneration.Custom)
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
                CustomGameManagerName + filenameExtension);

            if (!File.Exists(filePath)) return false;

            CustomGameManagerScript = File.ReadAllText(filePath);
            
            return true;
        }

        private void ReloadGameManagerIfNeeded()
        {
            if (_gameManagerType != GameManagerType.Custom)
                return;
            
            if (TryReloadGameManagerScript())
            {
                _ = AlertDisplayer.Instance.ShowAlert(
                    new Alert("Custom randomizer load failed!",
                        $"Randomizer {CustomGameManagerName} couldn't be found.",
                        AlertType.Error));
            }
            else
            {
                _ = AlertDisplayer.Instance.ShowAlert(
                    new Alert("Custom randomizer loaded!",
                        $"Randomizer {CustomGameManagerName} was loaded into game settings.",
                        AlertType.Success));
            }
        }
    }
}