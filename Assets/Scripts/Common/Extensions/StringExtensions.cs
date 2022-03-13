using System.Text;

namespace Blockstacker.Common.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Formats CamelCase strings by adding spaces after capital letters.
        /// </summary>
        public static string FormatCamelCase(this string input)
        {
            var output = new StringBuilder(input);
            for (var i = 1; i < output.Length; i++) {
                if (!char.IsUpper(output[i]) &&
                    (!char.IsDigit(output[i]) ||
                    char.IsDigit(output[i - 1]))) continue;
                output.Insert(i, ' ');
                i++;
            }

            return output.ToString();
        }
    }
}