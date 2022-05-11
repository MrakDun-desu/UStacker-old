using System;
using System.Collections.Generic;
using Blockstacker.GlobalSettings.Enums;
using TMPro;
using UnityEngine;

namespace Blockstacker.GlobalSettings.Changers
{
    public class DelayDasOnChanger : AppSettingChangerBase<DelayDasOn>
    {
        [SerializeField] private List<TMP_Text> _optionNames = new();

        private static DelayDasOn[] Values => new[]
        {
            DelayDasOn.Placement,
            DelayDasOn.Rotation
        };

        private static string[] ShownValues => new[]
        {
            "Placement",
            "Rotation"
        };
        
        private new void OnValidate()
        {
            base.OnValidate();

            for (var i = 0; i < ShownValues.Length; i++)
            {
                _optionNames[i].text = ShownValues[i];
            }
        }

        public void SetPlacement(bool value)
        {
            const DelayDasOn mask = ~DelayDasOn.Placement;
            var newValue = AppSettings.Handling.DelayDasOn & mask;
            if (value)
                AppSettings.Handling.DelayDasOn = newValue;
        }

        public void SetRotation(bool value)
        {
            const DelayDasOn mask = ~DelayDasOn.Rotation;
            var newValue = AppSettings.Handling.DelayDasOn & mask;
            if (value)
                AppSettings.Handling.DelayDasOn = newValue;
        }
    }
}