using System.Text;
using OwaspHeaders.Core.Helpers;
using OwaspHeaders.Core.Enums;

namespace OwaspHeaders.Core.Models
{
    public class XFrameOptionsConfiguration : IConfigurationBase
    {
        public XFrameOptions OptionValue { get; set; }
        public string AllowFromDomain { get; set; }

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
            var stringBuilder = new StringBuilder();
            switch (OptionValue)
            {
                case XFrameOptions.Deny:
                    stringBuilder.Append("DENY");
                    break;
                case XFrameOptions.Sameorigin:
                    stringBuilder.Append("SAMEORIGIN");
                    break;
                case XFrameOptions.Allowfrom:
                    if (string.IsNullOrWhiteSpace(AllowFromDomain))
                    {
                        ArgumentExceptionHelper.RaiseException(nameof(AllowFromDomain));
                    }
                    stringBuilder.Append($"ALLOW-FROM({AllowFromDomain})");
                    break;
                case XFrameOptions.AllowAll:
                    stringBuilder.Append("ALLOWALL");
                    break;
            }

            return stringBuilder.ToString();
        }
    }
}
