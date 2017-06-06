using System.Collections.Generic;
using System.Text;

namespace OwaspHeaders.Core.Models
{
    /// <summary>
    /// Represents the Public Key Pinning Extension for HTTP configuration
    /// </summary>
    public class HPKPConfiguration
    {
        /// <summary>
        /// The quoted string is the Base64 encoded Subject Public Key Information (SPKI) fingerprint
        /// </summary>
        public List<string> PinSha256 { get; set; }

        /// <summary>
        /// The time, in seconds, that the browser should remember that this site is only to be accessed using one of the pinned keys
        /// </summary>
        public int MaxAge { get; set; }

        /// <summary>
        /// (OPTIONAL) Whether this rule applies to all of the site's subdomains as well
        /// </summary>
        public bool IncludeSubDomains { get; set; }

        /// <summary>
        /// (OPTIONAL) The URL which pin validation failures are reported to
        /// </summary>
        public string ReportUri { get; set; }

        public HPKPConfiguration()
        {
            PinSha256 = new List<string>
            {
                "d6qzRu9zOECb90Uez27xWltNsj0e1Md7GkYYkVoZWmM="
            } ;
            ReportUri = "http://example.com/pkp-report";
            MaxAge = 10000;
            IncludeSubDomains = true;
        }

        /// <summary>
        /// Builds the HTTP header value
        /// </summary>
        /// <returns>A string representing the HTTP header value</returns>
        public string BuildHeaderValue()
        {
            //report-uri="http://example.com/pkp-report"; max-age=10000; includeSubDomains
            var stringBuilder = new StringBuilder();
            foreach (var pinSha256 in PinSha256)
            {
                stringBuilder.Append("pin-sha256=");
                stringBuilder.Append(pinSha256);
                stringBuilder.Append(";");
            }
            stringBuilder.Append("report-url=");
            stringBuilder.Append(ReportUri);
            stringBuilder.Append("max-age=");
            stringBuilder.Append(MaxAge);
            stringBuilder.Append(IncludeSubDomains ? "; includeSubDomains" : string.Empty);

            return stringBuilder.ToString();
        }
    }
}
