using System.Globalization;
using System.Linq;
using System.Text;

namespace Blockstacker.Common.Extensions
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

        public static string RemoveDiacritics(this string input)
        {
            var normalized = input.Normalize(NormalizationForm.FormKD);
            var builder = new StringBuilder(normalized.Length);

            foreach (var c in normalized.Select(character =>
                             new {character, unicodeCat = CharUnicodeInfo.GetUnicodeCategory(character)})
                         .Where(t => t.unicodeCat != UnicodeCategory.NonSpacingMark)
                         .Select(t => t.character))
                builder.Append(c);

            return builder.ToString().Normalize(NormalizationForm.FormC);
        }
    }
}