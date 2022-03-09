using System;

namespace Blockstacker.AppSettings.SettingGroups
{
    [Serializable]
    public class GameplaySettings
    {
        public float BoardVisibility = .8f;
        public float GridVisibility = .6f;
        public float GhostPieceVisibility = .5f;
        public float BoardZoom = 1;
        public bool CtrlScrollToChangeBoardZoom = false;
        public bool ScrollToChangeVolume = true;
        public bool DragMiddleButtonToRepositionBoard = false;
        public bool ColorGhostPiece = false;
        public bool MatrixBounce = true;
        public bool ShowActionText = true;
        public bool ShowNotFocusedWarning = true;
    }
}