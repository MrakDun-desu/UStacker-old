using System.IO;
using UnityEngine;

namespace Blockstacker.Common
{
    public static class CustomizationPaths
    {
        public static string GlobalSettings => Path.Combine(Application.persistentDataPath, "appSettings.json");
        // currenly unused
        public static string InputPresets => Path.Combine(Application.persistentDataPath, "inputPresets");
        private static string RulesCustomization => Path.Combine(Application.persistentDataPath, "rulesCustomization");
        public static string GameSettingsPresets => Path.Combine(Application.persistentDataPath, "gameSettingsPresets");
        public static string Randomizers => Path.Combine(RulesCustomization, "randomizers");
        public static string GarbageGenerators => Path.Combine(RulesCustomization, "garbageGenerators");
        public static string StatCounters => Path.Combine(RulesCustomization, "statCounters");
        public static string GameManagers => Path.Combine(RulesCustomization, "gameManagers");
        public static string RotationSystems => Path.Combine(RulesCustomization, "rotationSystems");
        
        private static string StyleCustomization => Path.Combine(Application.persistentDataPath, "styleCustomization");
        public static string BackgroundPacks => Path.Combine(StyleCustomization, "backgroundPacks");
        
        public static string Skins => Path.Combine(StyleCustomization, "skins");
        public static string SkinConfiguration => "skinConfig.json";

        public static string SoundPacks => Path.Combine(StyleCustomization, "soundPacks");
    }
}