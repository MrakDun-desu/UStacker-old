using UnityEngine.UIElements;

namespace Blockstacker.Common.UIToolkit
{
    public class BsDropdownField : BaseField<string>
    {
        public new class UxmlTraits : BaseField<string>.UxmlTraits
        {
        }

        public new class UxmlFactory : UxmlFactory<BsDropdownField, UxmlTraits>
        {
        }

        private BsDropdownField(string label, VisualElement visualInput) : base(label, visualInput)
        {
        }

        public BsDropdownField() : this("Label", new VisualElement())
        {
        }
    }
}