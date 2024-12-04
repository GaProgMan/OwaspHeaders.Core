namespace OwaspHeaders.Core.Models;

public class XFrameOptionsConfiguration : IConfigurationBase
{
    private XFrameOptions OptionValue { get; }
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
                return "DENY";
            case XFrameOptions.Sameorigin:
                return "SAMEORIGIN";
            case XFrameOptions.Allowfrom:
                HeaderValueGuardClauses.StringCannotBeNullOrWhitsSpace(AllowFromDomain, nameof(AllowFromDomain));
                return $"ALLOW-FROM({AllowFromDomain})";
            case XFrameOptions.AllowAll:
                return "ALLOWALL";
        }
        // We should never hit this return statement. It is included here
        // as the method NEEDs to return something.
        return ";";
    }
}
