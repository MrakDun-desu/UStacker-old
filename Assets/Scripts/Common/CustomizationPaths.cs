using System.IO;
using UnityEngine;

namespace Blockstacker.Common
{
    public static class CustomizationPaths
    {
        public static string GlobalSettings => Path.Combine(Application.persistentDataPath, "appSettings.json");
        public static string InputPresets => Path.Combine(Application.persistentDataPath, "inputPresets");
        private static string RulesCustomization => Path.Combine(Application.persistentDataPath, "rulesCustomization");
        public static string Randomizers => Path.Combine(RulesCustomization, "randomizers");
        public static string CheeseGenerators => Path.Combine(RulesCustomization, "garbageGenerators");
        public static string ScoringSystems => Path.Combine(RulesCustomization, "scoringSystems");
        public static string RotationSystems => Path.Combine(RulesCustomization, "rotationSystems");
        
        public static string StyleCustomization => Path.Combine(Application.persistentDataPath, "styleCustomization");
        public static string BackgroundPacks => Path.Combine(StyleCustomization, "backgroundPacks");
        
        public static string Skins => Path.Combine(StyleCustomization, "skins");
        public static string SkinConfiguration => "skinConfig.json";

        public static string SoundPacks => Path.Combine(StyleCustomization, "soundPacks");
        public static string Music => "music";
        public static string SoundEffects => "soundEffects";
        public static string SoundEffectScript => "soundEffects.lua";
        public static string MusicConfFile => "musicConfig.json";
    }
}