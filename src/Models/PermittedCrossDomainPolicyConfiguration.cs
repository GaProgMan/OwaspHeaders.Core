using OwaspHeaders.Core.Enums;
using OwaspHeaders.Core.Helpers;

namespace OwaspHeaders.Core.Models
{   
    public class PermittedCrossDomainPolicyConfiguration : IConfigurationBase
    {
        
        public XPermittedCrossDomainOptionValue xPermittedCrossDomainOptionValue { get; set; }

        protected PermittedCrossDomainPolicyConfiguration(){ }

        public PermittedCrossDomainPolicyConfiguration(
            XPermittedCrossDomainOptionValue permittedCrossDomainOptionValue)
        {
            xPermittedCrossDomainOptionValue = permittedCrossDomainOptionValue;
        }

        public string BuildHeaderValue()
        {
            switch (xPermittedCrossDomainOptionValue)
            {
                case XPermittedCrossDomainOptionValue.none:
                    return "none;";
                case XPermittedCrossDomainOptionValue.masterOnly:
                    return "master-only;";
                case XPermittedCrossDomainOptionValue.byContentType:
                    return "by-content-type;";
                case XPermittedCrossDomainOptionValue.byFtpFileType:
                    return "by-ftp-file-type;";
                case XPermittedCrossDomainOptionValue.all:
                    return "all;";
                default:
                    ArgumentExceptionHelper.RaiseException(nameof(xPermittedCrossDomainOptionValue));
                    break;
            }
            return ";";
        }
    }
}