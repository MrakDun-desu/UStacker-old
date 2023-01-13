using System;
using UnityEngine;

namespace UStacker.GlobalSettings.Groups
{
    [Serializable]
    public record GameplaySettings
    {
        // backing fields
        private float _boardVisibility = .8f;
        private float _boardZoom = 1f;
        private float _ghostPieceVisibility = .5f;
        private float _gridVisibility = .6f;
        private float _warningPieceTreshhold = 2f;
        private bool _pauseSingleplayerGamesOutOfFocus = true;
        public event Action<bool> PauseSingleplayerGamesOutOfFocusChanged;


        public float BoardVisibility
        {
            get => _boardVisibility;
            set => _boardVisibility = Mathf.Clamp(value, 0, 1);
        }
        public float GridVisibility
        {
            get => _gridVisibility;
            set => _gridVisibility = Mathf.Clamp(value, 0, 1);
        }

        public float GhostPieceVisibility
        {
            get => _ghostPieceVisibility;
            set => _ghostPieceVisibility = Mathf.Clamp(value, 0, 1);
        }

        public float BoardZoom
        {
            get => _boardZoom;
            set => _boardZoom = Mathf.Clamp(value, .01f, 2f);
        }

        public float WarningPieceTreshhold
        {
            get => _warningPieceTreshhold;
            set => _warningPieceTreshhold = Mathf.Clamp(value, -4f, 10f);
        }

        public bool PauseSingleplayerGamesOutOfFocus
        {
            get => _pauseSingleplayerGamesOutOfFocus;
            set
            {
                if (_pauseSingleplayerGamesOutOfFocus == value) return;
                _pauseSingleplayerGamesOutOfFocus = value;
                PauseSingleplayerGamesOutOfFocusChanged?.Invoke(value);
            }
        }

        public bool CtrlScrollToChangeBoardZoom { get; set; }

        public bool DragMiddleButtonToRepositionBoard { get; set; }

        public bool ColorGhostPiece { get; set; } = true;

        public bool ShowNotFocusedWarning { get; set; } = true;

        public string ReplayNamingFormat { get; set; } = "BSReplay_{GameType}_{Timestamp}";

        // not shown in the game menu
        public Vector2 BoardOffset { get; set; }
    }
}