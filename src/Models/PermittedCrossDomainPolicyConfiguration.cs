namespace OwaspHeaders.Core.Models;

public class PermittedCrossDomainPolicyConfiguration : IConfigurationBase
{

    public XPermittedCrossDomainOptionValue XPermittedCrossDomainOptionValue { get; init; }

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
