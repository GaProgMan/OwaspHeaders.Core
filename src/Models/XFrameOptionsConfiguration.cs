namespace OwaspHeaders.Core.Models;

public class XFrameOptionsConfiguration : IConfigurationBase
{
    public XFrameOptions OptionValue { get; }

    /// <remarks>
    /// Only meaningful for <see cref="XFrameOptions.Allowfrom"/>, and null for every other option,
    /// which is why <see cref="BuildHeaderValue"/> guards it before use.
    /// </remarks>
    public string? AllowFromDomain { get; init; }

    public XFrameOptionsConfiguration(XFrameOptions xFrameOption, string? allowFromDomain)
    {
        OptionValue = xFrameOption;
        AllowFromDomain = allowFromDomain;
    }

    /// <summary>
    /// Builds the HTTP header value
    /// </summary>
    /// <returns>A string representing the HTTP header value</returns>
    public string BuildHeaderValue()
    {
        switch (OptionValue)
        {
            case XFrameOptions.Deny:
                return "deny";
            case XFrameOptions.Sameorigin:
                return "sameorigin";
            case XFrameOptions.Allowfrom:
                HeaderValueGuardClauses.StringCannotBeNullOrWhiteSpace(AllowFromDomain, nameof(AllowFromDomain));
                return $"allow-from: ({AllowFromDomain})";
            case XFrameOptions.AllowAll:
                return "allowall";
        }
        // We should never hit this return statement. It is included here
        // as the method NEEDs to return something.
        return ";";
    }
}
