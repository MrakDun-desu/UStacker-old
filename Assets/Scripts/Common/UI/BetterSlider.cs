
/************************************
BetterSlider.cs -- created by Marek Dančo (xdanco00)
*************************************/
using System;
using UnityEngine;
using UnityEngine.UI;

namespace UStacker.Common.UI
{
    [RequireComponent(typeof(Slider))]
    public class BetterSlider : MonoBehaviour
    {
        [SerializeField] private float _minValue;
        [SerializeField] private float _maxValue = 100;

        private Slider _slider;

        public float MinValue
        {
            set => _minValue = value;
        }

        public float MaxValue
        {
            set => _maxValue = value;
        }

        public float Range
        {
            get
            {
                if (_slider != null) return _slider.maxValue;
                return default;
            }
            set
            {
                if (_slider != null) _slider.maxValue = value;
            }
        }

        private float RealRange => _maxValue - _minValue;

        private void Awake()
        {
            _slider = GetComponent<Slider>();
            _slider.onValueChanged.AddListener(val => ValueChanged?.Invoke(val));
        }

        public event Action<float> ValueChanged;

        public float GetRealValue()
        {
            return _slider.value / Range * RealRange + _minValue;
        }

        public void SetRealValue(float value)
        {
            value -= _minValue;
            _slider.SetValueWithoutNotify(value * Range / RealRange);
        }
    }
}
/************************************
end BetterSlider.cs
*************************************/
