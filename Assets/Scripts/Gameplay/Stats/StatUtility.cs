using System;
using System.Globalization;
using UStacker.Common.Extensions;
using JetBrains.Annotations;
using UnityEngine;

namespace UStacker.Gameplay.Stats
{
    public class StatUtility
    {
        private readonly GameTimer _timer;

        public StatUtility(GameTimer timer)
        {
            _timer = timer;
        }

        private string FormatNumberInternal(double num, int decimals)
        {
            const string infinityString = "INF";
            if (double.IsInfinity(num)) return infinityString;

            const int MIN_ROUNDABLE_DECIMALS = 0;
            const int MAX_ROUNDABLE_DECIMALS = 15;

            decimals = Mathf.Clamp(decimals, MIN_ROUNDABLE_DECIMALS, MAX_ROUNDABLE_DECIMALS);

            if (double.IsNaN(num)) num = 0;
            var output = Math.Round(num, decimals).ToString(CultureInfo.InvariantCulture);

            if (decimals == 0) return output;

            var dotIndex = output.LastIndexOfAny(new[]
            {
                '.',
                ','
            });
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
        public string GetFormattedTime()
        {
            return FormatTime(GetCurrentTime());
        }

        [UsedImplicitly]
        public double GetCurrentTime()
        {
            return _timer.CurrentTime;
        }

        [UsedImplicitly]
        public string FormatTime(object seconds)
        {
            return Convert.ToDouble(seconds).FormatAsTime();
        }
    }
}