using System;
using Blockstacker.GameSettings.Enums;

namespace Blockstacker.GameSettings.SettingGroups
{
    [Serializable]
    public class RulesSettings
    {
        // General
        public RandomBagType RandomBagType = RandomBagType.SevenBag;
        public AllowedSpins AllowedSpins = AllowedSpins.TSpins;
        public bool UseRandomSeed = true;
        public int SpecificSeed = 0;
        public uint NextPieceCount = 5;

        // Controls
        public bool Allow180Spins = true;
        public bool AllowHardDrop = true;
        public bool AllowHold = true;
        public KickTable KickTable = KickTable.SRS;
        public string CustomKickTableName = "";
        public float PiecePlacedDelay = 0;
        public float LineClearDelay = 0;
        public OnTouchGround OnTouchGround = OnTouchGround.LimitedTime;
        public float OnTouchGroundAmount = 5;
        public bool ShowGhostPiece = true;

        // Gravity and levelling
        public float Gravity = .02f;
        public float LockDelay = .5f;
        public bool UseLevelling = true;
        public bool UseMasterLevels = true;
        public uint StartingLevel = 1;
        public bool UseCustomLevellingScript = false;
        public string CustomLevellingScriptName = "";

        // Board dimensions
        public uint BoardHeight = 22;
        public uint BoardWidth = 10;
        public uint LethalHeight = 20;
        public uint PieceSpawnHeight = 22;
        public TopoutCondition TopoutCondition = TopoutCondition.LethalHeightLoose;
        public bool AllowClutchClears = true;

        // Handling
        public float DelayedAutoShift = .125f;
        public float AutomaticRepeatRate = 0;
        public float SoftDropFactor = float.PositiveInfinity;
        public float DasCutDelay = 0;
        public bool UseDasCancelling = true;
        public bool UseDiagonalLock = false;

    }
}