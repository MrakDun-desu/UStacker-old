using System;
using UnityEngine;

namespace Blockstacker.GlobalSettings.Groups
{
    [Serializable]
    public record GameplaySettings
    {
        public float BoardVisibility = .8f;
        public float GridVisibility = .6f;
        public float GhostPieceVisibility = .5f;
        public float BoardZoom = 1;
        public float WarningPieceTreshhold = 2;
        public bool CtrlScrollToChangeBoardZoom;
        public bool ScrollToChangeVolume = true;
        public bool DragMiddleButtonToRepositionBoard;
        public bool ColorGhostPiece;
        public Vector2 BoardOffset = Vector2.zero;
        
        // Implement in future
        // public bool MatrixBounce = true;
        // public bool ShowNotFocusedWarning = true;
    }
}