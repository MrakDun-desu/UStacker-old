using UnityEngine.Scripting;
using UnityEngine.UIElements;

namespace Blockstacker.GlobalSettings.VisualElements
{
    public class HandlingSlider : VisualElement
    {
        private readonly SliderInt _slider;
        private readonly Label _label;

        public bool DisplayFrames { get; set; }
        public string ControlPath { get; set; }
        public string LabelText { get; set; }
        public float Multiplier { get; set; }
        public float Start { get; set; }
        public float End { get; set; }
        public float Step { get; set; }
        public double Value { get; set; }
        
        public HandlingSlider() : this(
            "Label",
            "ms",
            true,
            1000,
            0,
            100,
            1)
        {
        }

        public HandlingSlider(
            string label,
            string unit,
            bool displayFrames,
            float multiplier,
            float start,
            float end,
            float step) 
        {
            
            _slider = new SliderInt((int)start, (int)end, SliderDirection.Horizontal, step);
            Add(_slider);
            
            _label = new Label(label);
            Add(_label);
        }

        [Preserve]
        public new class UxmlFactory : UxmlFactory<HandlingSlider, UxmlTraits>
        {
        }

        [Preserve]
        public new class UxmlTraits : BaseFieldTraits<double, UxmlDoubleAttributeDescription>
        {
            private readonly UxmlBoolAttributeDescription _displayFrames = new() {defaultValue = true, name = "display-frames"};
            private readonly UxmlStringAttributeDescription _labelText = new() {defaultValue = "Label", name = "label-text"};
            private readonly UxmlStringAttributeDescription _controlPath = new() {name = "control-path"};
            private readonly UxmlFloatAttributeDescription _multiplier = new() {defaultValue = 1000, name = "multiplier"};
            private readonly UxmlFloatAttributeDescription _start = new() {defaultValue = 0, name = "start"};
            private readonly UxmlFloatAttributeDescription _end = new() {defaultValue = 100, name = "end"};
            private readonly UxmlFloatAttributeDescription _step = new() {defaultValue = 1, name = "step"};

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);

                if (ve is not HandlingSlider slider) return;
                slider.DisplayFrames = _displayFrames.GetValueFromBag(bag, cc);
                slider.LabelText = _labelText.GetValueFromBag(bag, cc);
                slider.ControlPath = _controlPath.GetValueFromBag(bag, cc);
                slider.Multiplier = _multiplier.GetValueFromBag(bag, cc);
                slider.Start = _start.GetValueFromBag(bag, cc);
                slider.End = _end.GetValueFromBag(bag, cc);
                slider.Step = _step.GetValueFromBag(bag, cc);
            }
        }

    }
}