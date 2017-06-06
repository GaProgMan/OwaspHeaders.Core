using System.Text;

namespace OwaspHeaders.Core.Models
{
    /// <summary>
    /// Represents the HTTP Strict Transport Security configuration
    /// </summary>
    public class HstsConfiguration
    {
        /// <summary>
        /// (OPTIONAL) Whether this rule applies to all of the site's subdomains as well
        /// </summary>
        public bool IncludeSubDomains { get; set; }

        /// <summary>
        /// The time, in seconds, that the browser should remember that this site is only to be accessed using HTTPS
        /// </summary>
        public int MaxAge { get; set; }

        public HstsConfiguration()
        {
            IncludeSubDomains = true;
            MaxAge = 31536000;
        }

        /// <summary>
        /// Builds the HTTP header value
        /// </summary>
        /// <returns>A string representing the HTTP header value</returns>
        public string BuilderHeaderValue()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append("max-age=");
            stringBuilder.Append(MaxAge);
            stringBuilder.Append(IncludeSubDomains ? "; includeSubDomains" : string.Empty);

            return stringBuilder.ToString();
        }
    }
}
