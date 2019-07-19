using System.Collections.Generic;
using System.Linq;
using System.Text;
using OwaspHeaders.Core.Enums;
using OwaspHeaders.Core.Models;

namespace OwaspHeaders.Core.Extensions
{
    public static class StringBuilderExtensions
    {
        private static List<string> UnquotedDirectiveValues = new List<string>
        {
            "blob", "*", "data"
        };
        
        private static string EmptySpace = " ";

        /// <summary>
        /// Used to build the concatenated string value for the given values
        /// </summary>
        /// <param name="stringBuilder">The <see cref="StringBuilder" /> to use</param>
        /// <param name="directiveName">The name of the CSP directive</param>
        /// <param name="directiveValues">A list of strings representing the directive values</param>
        /// <returns>The updated <see cref="StringBuilder" /> instance</returns>
        public static StringBuilder BuildValuesForDirective(this StringBuilder @stringBuilder,
            string directiveName, List<ContentSecurityPolicyElement> directiveValues)
        {
            if (!directiveValues.Any()) return stringBuilder;
            
            @stringBuilder.Append(directiveName);            
            if (directiveValues.Any(d => d.CommandType == CspCommandType.Directive))
            {
                @stringBuilder.Append(EmptySpace);

                var unquoted = directiveValues.Where(command => (command.CommandType == CspCommandType.Directive))
                        .Where(directive => UnquotedDirectiveValues.Contains(directive.DirectiveOrUri));
                var quoted = directiveValues.Except(unquoted);

                if (unquoted.Any())
                {
                    @stringBuilder.Append(string.Join(EmptySpace, unquoted.Select(directive => directive.DirectiveOrUri)));
                    @stringBuilder.Append(EmptySpace);
                }
                if (quoted.Any())
                {
                    @stringBuilder.Append(string.Join(EmptySpace, quoted.Select(directive => $"'{directive.DirectiveOrUri}'")));
                }
            }

            if (directiveValues.Any(d => d.CommandType == CspCommandType.Uri))
            {
                @stringBuilder.Append(EmptySpace);
                @stringBuilder.Append(string.Join(EmptySpace,
                    directiveValues.Where(command => (command.CommandType == CspCommandType.Uri))
                        .Select(e => e.DirectiveOrUri)));
            }
            @stringBuilder.Append(";");
            return stringBuilder;
        }
    }
}
