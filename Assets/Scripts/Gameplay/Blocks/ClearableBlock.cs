
/************************************
ClearableBlock.cs -- created by Marek Dančo (xdanco00)
*************************************/
using System;
using UnityEngine.Events;

namespace UStacker.Gameplay.Blocks
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
/************************************
end ClearableBlock.cs
*************************************/
