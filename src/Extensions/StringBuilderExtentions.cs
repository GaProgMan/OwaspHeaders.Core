using System.Collections.Generic;
using System.Linq;
using System.Text;
using OwaspHeaders.Core.Enums;
using OwaspHeaders.Core.Models;

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
            string directiveName, List<ContenSecurityPolicyElement> directiveValues)
        {
            if (!directiveValues.Any()) return stringBuilder;
            
            @stringBuilder.Append(directiveName);            
            if (directiveValues.Any(d => d.CommandType == CspCommandType.Directive))
            {
                @stringBuilder.Append(" ");
                @stringBuilder.Append(string.Join(" ",
                    directiveValues.Where(command => (command.CommandType == CspCommandType.Directive))
                        .Select(directive => $"'{directive.DirectiveOrUri}'")));
            }

            if (directiveValues.Any(d => d.CommandType == CspCommandType.Uri))
            {
                @stringBuilder.Append(" ");
                @stringBuilder.Append(string.Join(" ",
                    directiveValues.Where(command => (command.CommandType == CspCommandType.Uri))
                        .Select(e => e.DirectiveOrUri)));
            }
            @stringBuilder.Append(";");
            return stringBuilder;
        }
    }
}
