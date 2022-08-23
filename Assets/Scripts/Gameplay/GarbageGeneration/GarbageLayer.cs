using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Blockstacker.Gameplay.Blocks;
using Blockstacker.Gameplay.Pieces;
using UnityEngine;
using UnityEngine.Pool;

namespace Blockstacker.Gameplay.GarbageGeneration
{
    public class GarbageLayer : MonoBehaviour, IBlockCollection
    {
        private readonly List<ClearableBlock> _blocks = new();
        public ReadOnlyCollection<ClearableBlock> Blocks => _blocks.AsReadOnly();
        public string Type => "garbage";
        public IEnumerable<Vector3> BlockPositions => _blocks.Select(block => block.transform.position);

        public ObjectPool<GarbageLayer> SourcePool;
        public ObjectPool<ClearableBlock> BlockSourcePool;

        public event Action BlocksAdded;

        public void AddBlock(ClearableBlock block)
        {
            block.SetBlockCollection(this);
            _blocks.Add(block);
            block.Cleared += OnBlockCleared;
        }

        private void OnBlockCleared(ClearableBlock block)
        {
            _blocks.Remove(block);
            block.Cleared -= OnBlockCleared;
            BlockSourcePool.Release(block);
            if (_blocks.Count <= 0)
                SourcePool.Release(this);
        }

        public void TriggerBlocksAdded()
        {
            foreach (var block in _blocks)
                block.RefreshSkins();
            
            BlocksAdded?.Invoke();
        }
    }
}