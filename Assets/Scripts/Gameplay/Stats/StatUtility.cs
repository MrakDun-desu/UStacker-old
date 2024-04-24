
/************************************
StatUtility.cs -- created by Marek Dančo (xdanco00)
*************************************/
using System;
using System.Globalization;
using JetBrains.Annotations;
using UnityEngine;
using UStacker.Common.Extensions;
using UStacker.Gameplay.Timing;

namespace UStacker.Gameplay.Stats
{
    public class StatUtility
    {
        private readonly GameTimer _timer;

        public StatUtility(GameTimer timer)
        {
            _timer = timer;
        }

        private static string FormatNumberInternal(double num, int decimals)
        {
            if (double.IsPositiveInfinity(num))
                return "+INF";
            if (double.IsNegativeInfinity(num))
                return "-INF";
            if (double.IsNaN(num))
                return "NaN";

            const int MIN_ROUNDABLE_DECIMALS = 0;
            const int MAX_ROUNDABLE_DECIMALS = 15;

            decimals = Mathf.Clamp(decimals, MIN_ROUNDABLE_DECIMALS, MAX_ROUNDABLE_DECIMALS);

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
            {
                missingZeroes = -(output.Length - decimals - dotIndex - 1);
            }

            return output.PadRight(output.Length + missingZeroes, '0');
        }

        [UsedImplicitly]
        public string FormatNumber(object num, object decimals = null)
        {
            var number = Convert.ToDouble(num);
            decimals ??= 2;
            return FormatNumberInternal(number, Convert.ToInt32(decimals));
        }

        [UsedImplicitly]
        public string GetFormattedTime()
        {
            return _timer.CurrentTime.FormatAsTime();
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

        [UsedImplicitly]
        public string FormatTime(object seconds, bool showMilliseconds)
        {
            return Convert.ToDouble(seconds).FormatAsTime(showMilliseconds);
        }
    }
}
/************************************
end StatUtility.cs
*************************************/
