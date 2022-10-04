using System;
using Blockstacker.Common.Attributes;
using UnityEngine.Events;

namespace Blockstacker.Common.UIToolkit
{
    [Serializable]
    public class UIElementsButtonBinding
    {
        [ReadOnly]
        public string ButtonName;
        public bool IsEnabled = true;
        public UnityEvent OnClick;

        public UIElementsButtonBinding(string buttonName)
        {
            ButtonName = buttonName;
        }
    }
}