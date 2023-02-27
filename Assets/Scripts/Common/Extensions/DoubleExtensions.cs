using System;

namespace UStacker.Common.Extensions
{
    public static class DoubleExtensions
    {
        public static string FormatAsTime(this double input)
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
                return $"{s}.{ms}";

            s = s.PadLeft(sLength, paddingChar);

            if (timeSpan.TotalHours < 1)
                return $"{m}:{s}.{ms}";

            m = m.PadLeft(mLength, paddingChar);

            return $"{h}:{m}:{s}.{ms}";
        }
    }
}