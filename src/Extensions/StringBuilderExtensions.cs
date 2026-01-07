using System.Linq;

namespace OwaspHeaders.Core.Extensions;

public static class StringBuilderExtensions
{
    private const char EmptySpace = ' ';

    /// <summary>
    /// This method is adapted from the following Stack Overflow answer:
    ///     https://stackoverflow.com/a/24769702/1143474
    /// It trims empty spaces from the end of an instance of the <seealso cref="StringBuilder"/> class
    /// </summary>
    /// <param name="sb"></param>
    /// <returns></returns>
    public static StringBuilder TrimEnd(this StringBuilder sb)
    {
        if (sb == null || sb.Length == 0)
        {
            return sb;
        }

        int i = sb.Length - 1;
        for (; i >= 0; i--)
        {
            if (!char.IsWhiteSpace(sb[i]))
            {
                break;
            }
        }

        if (i < sb.Length - 1)
        {
            sb.Length = i + 1;
        }

        return sb;
    }

    public static StringBuilder RemoveTrailingCharacter(this StringBuilder input, char toRemove)
    {
        if (input == null || input.Length == 0)
        {
            return input;
        }

        if (input[input.Length - 1] == toRemove)
        {
            input.Length--;
        }
        return input;
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
        if (directiveValues.Count == 0)
        {
            return stringBuilder;
        }

        stringBuilder.Append(directiveName);
        if (directiveValues.Any(d => d.CommandType == CspCommandType.Directive))
        {
            var directives = directiveValues.
                Where(command => command.CommandType == CspCommandType.Directive);
            stringBuilder.Append(EmptySpace);

            var contentSecurityPolicyElements = directives.ToList();
            if (contentSecurityPolicyElements.Count != 0)
            {
                stringBuilder.Append(string.Join(EmptySpace,
                    contentSecurityPolicyElements.Select(directive => $"'{directive.DirectiveOrUri}'")));
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
        stringBuilder.Append(';');
        return stringBuilder;
    }
}
