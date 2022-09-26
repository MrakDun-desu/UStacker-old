using System;
using System.Globalization;
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
            const char paddingChar = '0';
            const int msLength = 3;
            const int sLength = 2;
            const int mLength = 2;

            var timeSpan = TimeSpan.FromSeconds(seconds);

            var ms = timeSpan.Milliseconds.ToString();
            var s = timeSpan.Seconds.ToString();
            var m = timeSpan.Minutes.ToString();
            var h = ((int) timeSpan.TotalHours).ToString();

            ms = ms.PadLeft(msLength, paddingChar);

            if (timeSpan.Minutes == 0 && timeSpan.TotalHours < 1)
                return $"{s}.{ms}";

            s = s.PadLeft(sLength, paddingChar);

            if (timeSpan.TotalHours < 1)
                return $"{m}:{s}.{ms}";

            m = m.PadLeft(mLength, paddingChar);

            return $"{h}:{m}:{s}.{ms}";
        }

        [UsedImplicitly]
        public string FormatNumber(double num, int decimals = 2)
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
        public string GetFormattedTime() => FormatTime(GetCurrentTime());

        [UsedImplicitly]
        public double GetCurrentTime() => _timer.CurrentTime;
    }
}