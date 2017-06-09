using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace OwaspHeaders.Core.Extensions
{
    public static class StringBuilderExtentions
    {
        /// <summary>
        /// Used to build the concatenated string value for the given values
        /// </summary>
        /// <param name="@stringBuilder">The <see cref="StringBuilder" /> to use</param>
        /// <param name="directiveName">The name of the CSP directive</param>
        /// <param name="directiveValues">A list of strings representing the directive values</param>
        /// <returns>The updated <see cref="StringBuilder" /> instance</returns>
        public static StringBuilder BuildValuesForDirective(this StringBuilder @stringBuilder, string directiveName, List<string> directiveValues)
        {
            if (directiveValues.Any())
            {
                @stringBuilder.Append(directiveName);
                directiveValues.Select(s => @stringBuilder.Append($"'{s}' "));
                @stringBuilder.Append(";");
            }
            return stringBuilder;
        }
    }
}