using System;
using System.ComponentModel;
using Blockstacker.Common.Attributes;
using UnityEngine;

namespace Blockstacker.GlobalSettings.Groups
{
    [Serializable]
    public record GameplaySettings
    {
        
        [MinRestraint(0, true)]
        [MaxRestraint(1, true)]
        public float BoardVisibility = .8f;
        
        [MinRestraint(0, true)]
        [MaxRestraint(1, true)]
        public float GridVisibility = .6f;
        
        [MinRestraint(0, true)]
        [MaxRestraint(1, true)]
        public float GhostPieceVisibility = .5f;
        
        [MinRestraint(0, true)]
        [MaxRestraint(2, false)]
        public float BoardZoom = 1;
        
        [MinRestraint(0, true)]
        [MaxRestraint(10, true)]
        [Tooltip("How many blocks under lethal height to start showing warning piece")]
        public float WarningPieceTreshhold = 2;
        
        [Description("Control + Scroll to change Board Zoom")]
        [Tooltip("If set, you can change board zoom while playing the game")]
        public bool CtrlScrollToChangeBoardZoom;
       
        // Not yet implemented
        [Description("Scroll to change Volume")]
        [Tooltip("If set, you can change volume while playing the game")]
        public bool ScrollToChangeVolume = true;
        
        [Description("Drag Middle Mouse button to reposition Board")]
        [Tooltip("If set, you can reposition board while playing the game")]
        public bool DragMiddleButtonToRepositionBoard;
        
        [Tooltip("If set, ghost piece will be colored depending on your active piece")]
        public bool ColorGhostPiece;
        
        [Tooltip("If set, will show a warning when you're playing but not focused on game")]
        public bool ShowNotFocusedWarning = true;
        
        // not shown in the game menu
        public Vector2 BoardOffset = Vector2.zero;
    }
}