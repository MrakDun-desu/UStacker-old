using System;
using UnityEngine.Events;

namespace Blockstacker.Gameplay.Blocks
{
    public class ClearableBlock : BlockBase
    {
        public UnityEvent OnCleared;
        public event Action<ClearableBlock> Cleared;

        public void Clear()
        {
            OnCleared.Invoke();
            Cleared?.Invoke(this);
        }
    }
}