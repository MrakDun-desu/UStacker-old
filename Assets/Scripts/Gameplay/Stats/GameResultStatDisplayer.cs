using System;
using System.Globalization;
using Blockstacker.Common.Extensions;
using TMPro;
using UnityEngine;

namespace Blockstacker.Gameplay.Stats
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
            value = Math.Round(value, 2);
            _valueText.text = formatAsTime ? value.FormatAsTime() : value.ToString(CultureInfo.InvariantCulture);
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