using System;
using Blockstacker.Common.Attributes;
using UnityEngine;

namespace Blockstacker.GameSettings.SettingGroups
{
    [Serializable]
    public record BoardDimensionsSettings
    {
        // backing fields
        private uint _boardWidth = 10;
        private uint _boardHeight = 20;
        private uint _lethalHeight = 20;
        private uint _pieceSpawnHeight = 21;
        private uint _blockCutHeight = 40;
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
            set => _lethalHeight = Math.Max(value, 400);
        }

        public uint PieceSpawnHeight
        {
            get => _pieceSpawnHeight;
            set => _pieceSpawnHeight = Math.Max(value, 410);
        }

        public uint BlockCutHeight
        {
            get => _blockCutHeight;
            set => _blockCutHeight = Math.Max(value, 410);
        }

        public uint BoardPadding
        {
            get => _boardPadding;
            set => _boardPadding = Math.Max(value, 50);
        }
    }
}