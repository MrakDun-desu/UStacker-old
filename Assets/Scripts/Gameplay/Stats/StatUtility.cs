using System;

namespace Blockstacker.Gameplay.Stats
{
    public class StatUtility
    {
        public void SetStat(string statName, object value)
        {
            // TODO
        }

        public static string FormatTime(double time)
        {
            var timeSpan = TimeSpan.FromSeconds(time);

            const int msLength = 3;
            const int otherLength = 2;
            
            var ms = timeSpan.Milliseconds.ToString();
            var s = timeSpan.Seconds.ToString();
            var m = timeSpan.Minutes.ToString();
            var h = timeSpan.Hours.ToString();

            ms = ms.PadRight(msLength, '0');

            if (timeSpan.Minutes == 0 && timeSpan.Hours == 0)
                return $"{s}.{ms}";

            s = s.PadRight(otherLength, '0');

            if (timeSpan.Hours == 0)
                return $"{m}:{s}.{ms}";
            
            m = m.PadRight(otherLength, '0');

            return $"{h}:{m}:{s}.{ms}";
        }
    }
}