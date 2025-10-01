namespace OwaspHeaders.Core.Models;

/// <summary>
/// Configuration for the Clear-Site-Data header
/// </summary>
public class ClearSiteDataConfiguration : IConfigurationBase
{
    /// <summary>
    /// The directive options to include in the Clear-Site-Data header
    /// </summary>
    public ClearSiteDataOptions[] DirectiveOptions { get; init; } = [];

    /// <summary>
    /// Protected parameterless constructor for deserialization
    /// </summary>
    protected ClearSiteDataConfiguration() { }

    /// <summary>
    /// Initializes a new instance of the ClearSiteDataConfiguration class
    /// </summary>
    /// <param name="directiveOptions">The directive options to include</param>
    /// <exception cref="ArgumentException">Thrown when directiveOptions is null or empty</exception>
    public ClearSiteDataConfiguration(params ClearSiteDataOptions[] directiveOptions)
    {
        ObjectGuardClauses.ObjectCannotBeNull(directiveOptions, nameof(directiveOptions),
            $"{nameof(directiveOptions)} cannot be null");

        if (directiveOptions.Length == 0)
        {
            ArgumentExceptionHelper.RaiseException(nameof(directiveOptions));
        }

        DirectiveOptions = directiveOptions;
    }

    /// <summary>
    /// Builds the header value for the Clear-Site-Data header
    /// </summary>
    /// <returns>The formatted header value</returns>
    public string BuildHeaderValue()
    {
        if (Array.Exists(DirectiveOptions, opt => opt == ClearSiteDataOptions.wildcard))
        {
            return "\"*\"";
        }

        var builder = new StringBuilder();
        var distinctDirectives = new List<ClearSiteDataOptions>();
        foreach (var directive in DirectiveOptions)
        {
            if (directive != ClearSiteDataOptions.wildcard && !distinctDirectives.Contains(directive))
            {
                distinctDirectives.Add(directive);
            }
        }

        foreach (var directive in distinctDirectives)
        {
            if (builder.Length > 0)
            {
                builder.Append(',');
            }

            var directiveValue = directive switch
            {
                ClearSiteDataOptions.cache => "\"cache\"",
                ClearSiteDataOptions.cookies => "\"cookies\"",
                ClearSiteDataOptions.storage => "\"storage\"",
                _ => throw new ArgumentOutOfRangeException(nameof(directive), directive, "Unknown directive option")
            };

            builder.Append(directiveValue);
        }

        return builder.ToString();
    }
}
