using System;
using Blockstacker.Common.Attributes;
using UnityEngine.Events;

namespace Blockstacker.Common
{
    [Serializable]
    public class UiElementsButtonBinding
    {
        [ReadOnly]
        public string ButtonName;
        public bool IsEnabled = true;
        public UnityEvent OnClick;

        public UiElementsButtonBinding(string buttonName)
        {
            ButtonName = buttonName;
        }
    }
}