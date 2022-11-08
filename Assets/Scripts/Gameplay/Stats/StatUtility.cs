﻿using System;
using System.Globalization;
using Blockstacker.Common.Extensions;
using JetBrains.Annotations;
using UnityEngine;

namespace Blockstacker.Gameplay.Stats
{
    public class StatUtility
    {
        private readonly GameTimer _timer;

        public StatUtility(GameTimer timer)
        {
            _timer = timer;
        }

        [UsedImplicitly]
        public string FormatTime(double seconds)
        {
            return seconds.FormatAsTime();
        }

        [UsedImplicitly]
        public string FormatNumberInternal(double num, int decimals)
        {
            const string infinityString = "INF";
            if (double.IsInfinity(num)) return infinityString;

            const int MIN_ROUNDABLE_DECIMALS = 0;
            const int MAX_ROUNDABLE_DECIMALS = 15;
            
            decimals = Mathf.Clamp(decimals, MIN_ROUNDABLE_DECIMALS, MAX_ROUNDABLE_DECIMALS);
            
            if (double.IsNaN(num)) num = 0;
            var output = Math.Round(num, decimals).ToString(CultureInfo.InvariantCulture);

            if (decimals == 0) return output;

            var dotIndex = output.LastIndexOfAny(new[] {'.', ','});
            int missingZeroes;
            if (dotIndex == -1)
            {
                output += ".";
                missingZeroes = 2;
            }
            else
                missingZeroes = -(output.Length - decimals - dotIndex - 1);

            return output.PadRight(output.Length + missingZeroes, '0');
        }

        [UsedImplicitly]
        public string FormatNumber(object num, object decimals = null)
        {
            var number = Convert.ToDouble(num);
            return FormatNumberInternal(number, decimals is null ? 2 : Convert.ToInt32(decimals));
        }

        [UsedImplicitly]
        public string GetFormattedTime() => FormatTime(GetCurrentTime());

        [UsedImplicitly]
        public double GetCurrentTime() => _timer.CurrentTime;
    }
}