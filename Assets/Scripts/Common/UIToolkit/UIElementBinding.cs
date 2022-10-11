using System;
using Blockstacker.Common.Attributes;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Blockstacker.Common.UIToolkit
{
    [Serializable]
    public class UIElementBinding
    {
        [FormerlySerializedAs("ButtonName")] [ReadOnly]
        public string ControlName;
        public bool IsEnabled = true;
        public UnityEvent BoundAction;

        public UIElementBinding(string controlName)
        {
            ControlName = controlName;
        }
    }
}