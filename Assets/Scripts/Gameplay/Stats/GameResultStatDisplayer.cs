using System;
using System.Globalization;
using UStacker.Common.Extensions;
using TMPro;
using UnityEngine;

namespace UStacker.Gameplay.Stats
{
    public class GameResultStatDisplayer : MonoBehaviour
    {
        [SerializeField] private TMP_Text _nameText;
        [SerializeField] private TMP_Text _valueText;

        public void DisplayStat(string statName, uint value)
        {
            _nameText.text = statName;
            _valueText.text = value.ToString();
        }

        public void DisplayStat(string statName, double value, bool formatAsTime = false)
        {
            _nameText.text = statName;
            _valueText.text = formatAsTime
                ? value.FormatAsTime()
                : Math.Round(value, 2).ToString(CultureInfo.InvariantCulture);
        }

        public void DisplayStat(string statName, string value)
        {
            _nameText.text = statName;
            _valueText.text = value;
        }

        public void DisplayStat(string statName, long value)
        {
            _nameText.text = statName;
            _valueText.text = value.ToString();
        }
    }
}