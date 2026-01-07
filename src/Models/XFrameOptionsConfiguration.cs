namespace OwaspHeaders.Core.Models;

public class XFrameOptionsConfiguration : IConfigurationBase
{
    public XFrameOptions OptionValue { get; }
    public string AllowFromDomain { get; init; }

    /// <summary>
    /// Protected constructor, we can no longer create instances of this class without
    /// using the public constructor
    /// </summary>
    [ExcludeFromCodeCoverage]
    protected XFrameOptionsConfiguration() { }

    public XFrameOptionsConfiguration(XFrameOptions xFrameOption, string allowFromDomain)
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
                HeaderValueGuardClauses.StringCannotBeNullOrWhitsSpace(AllowFromDomain, nameof(AllowFromDomain));
                return $"allow-from: ({AllowFromDomain})";
            case XFrameOptions.AllowAll:
                return "allowall";
        }
        // We should never hit this return statement. It is included here
        // as the method NEEDs to return something.
        return ";";
    }
}
