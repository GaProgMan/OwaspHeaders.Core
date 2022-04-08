using System.Collections.Generic;
using System.Linq;
using System.Text;
using OwaspHeaders.Core.Enums;
using OwaspHeaders.Core.Models;

namespace OwaspHeaders.Core.Extensions
{
    public static class StringBuilderExtensions
    {
        private static string EmptySpace = " ";

        /// <summary>
        /// This method is adapted from the following Stack Overflow answer:
        ///     https://stackoverflow.com/a/24769702/1143474
        /// It trims empty spaces from the end of an instance of the <seealso cref="StringBuilder"/> class
        /// </summary>
        /// <param name="sb"></param>
        /// <returns></returns>
        private static StringBuilder TrimEnd(this StringBuilder sb)
        {
            if (sb == null || sb.Length == 0) return sb;

            int i = sb.Length - 1;
            for (; i >= 0; i--)
                if (!char.IsWhiteSpace(sb[i]))
                    break;

            if (i < sb.Length - 1)
                sb.Length = i + 1;

            return sb;
        }

        /// <summary>
        /// Used to build the concatenated string value for the given values
        /// </summary>
        /// <param name="stringBuilder">The <see cref="StringBuilder" /> to use</param>
        /// <param name="directiveName">The name of the CSP directive</param>
        /// <param name="directiveValues">A list of strings representing the directive values</param>
        /// <returns>The updated <see cref="StringBuilder" /> instance</returns>
        public static StringBuilder BuildValuesForDirective(this StringBuilder stringBuilder,
            string directiveName, List<ContentSecurityPolicyElement> directiveValues)
        {
            if (!directiveValues.Any()) return stringBuilder;

            stringBuilder.Append(directiveName);
            if (directiveValues.Any(d => d.CommandType == CspCommandType.Directive))
            {

                var directives = directiveValues.Where(command => command.CommandType == CspCommandType.Directive);
                var uris = directiveValues.Where(command => command.CommandType == CspCommandType.Uri);

                stringBuilder.Append(EmptySpace);

                if (directives.Any())
                {
                    stringBuilder.Append(string.Join(EmptySpace, directives.Select(directive => $"'{directive.DirectiveOrUri}'")));
                    stringBuilder.Append(EmptySpace);
                }
                if (uris.Any())
                {
                    stringBuilder.Append(string.Join(EmptySpace, uris.Select(directive => directive.DirectiveOrUri)));
                }
            }

            if (directiveValues.Any(d => d.CommandType == CspCommandType.Uri))
            {
                stringBuilder.Append(EmptySpace);
                stringBuilder.Append(string.Join(EmptySpace,
                    directiveValues.Where(command => (command.CommandType == CspCommandType.Uri))
                        .Select(e => e.DirectiveOrUri)));
            }

            stringBuilder.TrimEnd();
            stringBuilder.Append(";");
            return stringBuilder;
        }
    }
}
