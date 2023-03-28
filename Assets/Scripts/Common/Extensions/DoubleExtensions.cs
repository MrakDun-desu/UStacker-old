using System;

namespace UStacker.Common.Extensions
{
    public static class DoubleExtensions
    {
        public static string FormatAsTime(this double input, bool showMilliseconds = true)
        {
            const char paddingChar = '0';
            const int msLength = 3;
            const int sLength = 2;
            const int mLength = 2;

            var timeSpan = TimeSpan.FromSeconds(input);

            var ms = timeSpan.Milliseconds.ToString();
            var s = timeSpan.Seconds.ToString();
            var m = timeSpan.Minutes.ToString();
            var h = ((uint) timeSpan.TotalHours).ToString();

            ms = ms.PadLeft(msLength, paddingChar);

            if (timeSpan is {Minutes: 0, TotalHours: < 1})
                return showMilliseconds ? $"{s}.{ms}" : s;

            s = s.PadLeft(sLength, paddingChar);

            if (timeSpan.TotalHours < 1)
                return showMilliseconds ? $"{m}:{s}.{ms}" : $"{m}:{s}";

            m = m.PadLeft(mLength, paddingChar);

            return showMilliseconds ? $"{h}:{m}:{s}.{ms}" : $"{h}:{m}:{s}";
        }
    }
}