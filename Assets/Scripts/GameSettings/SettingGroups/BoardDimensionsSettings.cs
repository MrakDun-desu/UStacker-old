using System;
using UnityEngine;

namespace UStacker.GameSettings.SettingGroups
{
    [Serializable]
    public record BoardDimensionsSettings
    {
        // backing fields
        [SerializeField]
        private uint _boardWidth = 10;
        [SerializeField]
        private uint _boardHeight = 20;
        [SerializeField]
        private uint _lethalHeight = 20;
        [SerializeField]
        private uint _pieceSpawnHeight = 21;
        [SerializeField]
        private uint _blockCutHeight = 40;
        [SerializeField]
        private uint _boardPadding = 4;

        public uint BoardWidth
        {
            get => _boardWidth;
            set => _boardWidth = Math.Min(value, 200);
        }

        public uint BoardHeight
        {
            get => _boardHeight;
            set => _boardHeight = Math.Min(value, 400);
        }

        public uint LethalHeight
        {
            get => _lethalHeight;
            set => _lethalHeight = Math.Min(value, 400);
        }

        public uint PieceSpawnHeight
        {
            get => _pieceSpawnHeight;
            set => _pieceSpawnHeight = Math.Min(value, 410);
        }

        public uint BlockCutHeight
        {
            get => _blockCutHeight;
            set => _blockCutHeight = Math.Min(value, 410);
        }

        public uint BoardPadding
        {
            get => _boardPadding;
            set => _boardPadding = Math.Min(value, 50);
        }
    }
}