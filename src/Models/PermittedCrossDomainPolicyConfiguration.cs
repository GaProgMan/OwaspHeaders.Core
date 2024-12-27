namespace OwaspHeaders.Core.Models;

public class PermittedCrossDomainPolicyConfiguration : IConfigurationBase
{

    public XPermittedCrossDomainOptionValue XPermittedCrossDomainOptionValue { get; init; }

    /// <summary>
    /// Protected constructor, we can no longer create instances of this class without
    /// using the public constructor
    /// </summary>
    [ExcludeFromCodeCoverage]
    protected PermittedCrossDomainPolicyConfiguration() { }

    public PermittedCrossDomainPolicyConfiguration(
        XPermittedCrossDomainOptionValue permittedCrossDomainOptionValue)
    {
        XPermittedCrossDomainOptionValue = permittedCrossDomainOptionValue;
    }

    public string BuildHeaderValue()
    {
        switch (XPermittedCrossDomainOptionValue)
        {
            case XPermittedCrossDomainOptionValue.none:
                return "none";
            case XPermittedCrossDomainOptionValue.masterOnly:
                return "master-only";
            case XPermittedCrossDomainOptionValue.byContentType:
                return "by-content-type";
            case XPermittedCrossDomainOptionValue.byFtpFileType:
                return "by-ftp-file-type";
            case XPermittedCrossDomainOptionValue.all:
                return "all";
            default:
                ArgumentExceptionHelper.RaiseException(nameof(XPermittedCrossDomainOptionValue));
                break;
        }
        // We should never hit this return statement. It is included here
        // as the method NEEDs to return something.
        return ";";
    }
}
