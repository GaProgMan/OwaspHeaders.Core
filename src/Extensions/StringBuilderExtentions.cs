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
        /// <param name="stringBuilder">The <see cref="StringBuilder" /> to use</param>
        /// <param name="directiveName">The name of the CSP directive</param>
        /// <param name="directiveValues">A list of strings representing the directive values</param>
        /// <returns>The updated <see cref="StringBuilder" /> instance</returns>
        public static StringBuilder BuildValuesForDirective(this StringBuilder @stringBuilder,
            string directiveName, List<string> directiveValues)
        {
            if (!directiveValues.Any()) return stringBuilder;
            
            @stringBuilder.Append(directiveName);            
            @stringBuilder.Append(" ");
            @stringBuilder.Append(string.Join(" ", directiveValues));
            @stringBuilder.Append(";");
            return stringBuilder;
        }
    }
}
