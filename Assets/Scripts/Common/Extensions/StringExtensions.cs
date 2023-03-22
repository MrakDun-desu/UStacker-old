using System.Globalization;
using System.Linq;
using System.Text;

namespace UStacker.Common.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        ///     Formats CamelCase strings by adding spaces after capital letters.
        /// </summary>
        public static string FormatCamelCase(this string input)
        {
            var output = new StringBuilder(input);
            for (var i = 1; i < output.Length; i++)
            {
                if (!char.IsUpper(output[i]) &&
                    (!char.IsDigit(output[i]) ||
                     char.IsDigit(output[i - 1]))) continue;
                output.Insert(i, ' ');
                i++;
            }

            return output.ToString();
        }

        public static bool TryParseDouble(this string input, out double output)
        {
            switch (input.ToLower())
            {
                case "" :
                    output = 0;
                    return true;
                case "inf":
                case "+inf":
                    output = double.PositiveInfinity;
                    return true;
                case "-inf":
                    output = double.NegativeInfinity;
                    return true;
                default:
                    var parsedStr = input.Replace(',', '.');
                    return double.TryParse(parsedStr, NumberStyles.Float, CultureInfo.InvariantCulture, out output);
            }
        }

        public static bool TryParseFloat(this string input, out float output)
        {
            switch (input.ToLower())
            {
                case "" :
                    output = 0;
                    return true;
                case "inf":
                case "+inf":
                    output = float.PositiveInfinity;
                    return true;
                case "-inf":
                    output = float.NegativeInfinity;
                    return true;
                default:
                    var parsedStr = input.Replace(',', '.');
                    return float.TryParse(parsedStr, NumberStyles.Float, CultureInfo.InvariantCulture, out output);
            }
        }

        public static string RemoveDiacritics(this string input)
        {
            var normalized = input.Normalize(NormalizationForm.FormKD);
            var builder = new StringBuilder(normalized.Length);

            foreach (var c in normalized.Select(character =>
                             new
                             {
                                 character, unicodeCat = CharUnicodeInfo.GetUnicodeCategory(character)
                             })
                         .Where(t => t.unicodeCat != UnicodeCategory.NonSpacingMark)
                         .Select(t => t.character))
                builder.Append(c);

            return builder.ToString().Normalize(NormalizationForm.FormC);
        }
    }
}