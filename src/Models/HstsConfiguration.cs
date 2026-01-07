namespace OwaspHeaders.Core.Models;

/// <summary>
/// Represents the HTTP Strict Transport Security configuration
/// </summary>
public class HstsConfiguration : IConfigurationBase
{
    /// <summary>
    /// (OPTIONAL) Whether this rule applies to all the site's subdomains as well
    /// </summary>
    public bool IncludeSubDomains { get; }

    /// <summary>
    /// The time, in seconds, that the browser should remember that this site is only to be accessed using HTTPS
    /// </summary>
    public int MaxAge { get; }

    /// <summary>
    /// Protected constructor, we can no longer create instances of this
    /// class without using the public constructor
    /// </summary>
    [ExcludeFromCodeCoverage]
    protected HstsConfiguration() { }

    public HstsConfiguration(int maxAge, bool includeSubDomains)
    {
        MaxAge = maxAge;
        IncludeSubDomains = includeSubDomains;
    }

    /// <summary>
    /// Builds the HTTP header value
    /// </summary>
    /// <returns>A string representing the HTTP header value</returns>
    public string BuildHeaderValue()
    {
        var stringBuilder = new StringBuilder("max-age=");
        stringBuilder.Append(MaxAge);

        if (IncludeSubDomains)
        {
            stringBuilder.Append(";includeSubDomains");
        }

        return stringBuilder.ToString();
    }
}
