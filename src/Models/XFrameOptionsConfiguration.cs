using System;
using System.Text;
using OwaspHeaders.Core.Helpers;

namespace OwaspHeaders.Core.Models
{
    public class XFrameOptionsConfiguration : IConfigurationBase
    {
        public enum XFrameOptions { deny, sameorigin, allowfrom };
        public XFrameOptions OptionValue { get; set; }
        public string AllowFromDomain { get; set; }

        public XFrameOptionsConfiguration()
        {
            OptionValue = XFrameOptions.sameorigin;
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
                case XFrameOptions.deny:
                    stringBuilder.Append("deny");
                    break;
                case XFrameOptions.sameorigin:
                    stringBuilder.Append("sameorigin");
                    break;
                case XFrameOptions.allowfrom:
                    if (string.IsNullOrWhiteSpace(AllowFromDomain))
                    {
                        ArgumentExceptionHelper.RaiseException(nameof(AllowFromDomain));
                    }
                    stringBuilder.Append("allow-from: ");
                    stringBuilder.Append(AllowFromDomain);
                    break;
            }

            return stringBuilder.ToString();
        }
    }
}
