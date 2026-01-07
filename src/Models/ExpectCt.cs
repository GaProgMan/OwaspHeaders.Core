// The documentation for properties found in this class is taken from
// the announcement post for this header at Scott Helme's blog.
// Full details can be found at the folloing url:
// https://scotthelme.co.uk/a-new-security-header-expect-ct/

namespace OwaspHeaders.Core.Models;

public class ExpectCt : IConfigurationBase
{
    /// <summary>
    /// [OPTIONAL]
    /// The enforce directive controls whether the browser should enforce the
    /// policy or treat it as report-only mode. The directive has no value so
    /// you simply include it or not depending on whether you want the browser
    /// to enforce the policy or just report on it.
    /// </summary>
    public bool Enforce { get; }

    /// <summary>
    /// The max-age directive specifies the number of seconds that the browser
    /// should cache and apply the received policy for, whether enforced or
    /// report-only
    /// </summary>
    public int MaxAge { get; }

    /// <summary>
    /// The report-uri directive specifies where the browser should send reports
    /// if it does not receive valid CT information. This is specified as an
    /// absolute URI
    /// </summary>
    public string ReportUri { get; }

    /// <summary>
    /// Protected constructor, we can no longer create instances of this
    /// class without using the public constructor
    /// </summary>
    protected ExpectCt() { }

    public ExpectCt(string reportUri, int maxAge = 86400, bool enforce = false)
    {
        ReportUri = reportUri;
        MaxAge = maxAge;
        Enforce = enforce;
    }

    /// <summary>
    /// Builds the HTTP header value
    /// </summary>
    /// <returns>A string representing the HTTP header value</returns>
    public string BuildHeaderValue()
    {
        var stringBuilder = new StringBuilder("max-age=");
        stringBuilder.Append(MaxAge);

        if (Enforce)
        {
            stringBuilder.Append(", enforce");
        }

        if (!string.IsNullOrWhiteSpace(ReportUri))
        {
            stringBuilder.Append(", report-uri=");
            stringBuilder.Append($"\"{ReportUri}\"");
        }

        return stringBuilder.ToString();
    }
}
