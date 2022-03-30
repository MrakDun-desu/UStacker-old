using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Blockstacker.GlobalSettings.Changers
{
    [RequireComponent(typeof(Slider))]
    public class BetterSlider : MonoBehaviour
    {
        [SerializeField] private float _minValue;
        [SerializeField] private float _maxValue = 100;

        public float MinValue { set => _minValue = value; }
        public float MaxValue { set => _maxValue = value; }

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

        private Slider _slider;

        private void Awake()
        {
            _slider = GetComponent<Slider>();
        }

        public float GetRealValue() => _slider.value / Range * RealRange + _minValue;

        public void SetRealValue(float value)
        {
            value -= _minValue;
            _slider.SetValueWithoutNotify(value * Range / RealRange);
        }
    }
}