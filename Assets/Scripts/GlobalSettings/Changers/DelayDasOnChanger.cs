﻿using System.Collections.Generic;
using Blockstacker.GlobalSettings.Enums;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Blockstacker.GlobalSettings.Changers
{
    public class DelayDasOnChanger : AppSettingChangerBase<DelayDasOn>
    {
        [SerializeField] private List<TMP_Text> _optionNames = new();

        [SerializeField] private Toggle _placementToggle;
        [SerializeField] private Toggle _rotationToggle;

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
            
            _placementToggle.SetIsOnWithoutNotify(AppSettings.Handling.DelayDasOn.HasFlag(DelayDasOn.Placement));
            _rotationToggle.SetIsOnWithoutNotify(AppSettings.Handling.DelayDasOn.HasFlag(DelayDasOn.Rotation));
        }

        public void SetPlacement(bool value)
        {
            var oldValue = AppSettings.Handling.DelayDasOn;
            if (value)
                AppSettings.Handling.DelayDasOn = oldValue | DelayDasOn.Placement;
            else
                AppSettings.Handling.DelayDasOn = oldValue & ~DelayDasOn.Placement;
        }

        public void SetRotation(bool value)
        {
            var oldValue = AppSettings.Handling.DelayDasOn;
            if (value)
                AppSettings.Handling.DelayDasOn = oldValue | DelayDasOn.Rotation;
            else
                AppSettings.Handling.DelayDasOn = oldValue & ~DelayDasOn.Rotation;
        }
    }
}