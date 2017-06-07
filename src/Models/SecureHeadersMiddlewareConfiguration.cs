namespace OwaspHeaders.Core.Models
{
    public class SecureHeadersMiddlewareConfiguration : ISecureHeadersMiddlewareConfiguration
    {
        /// <summary>
        /// Indicates whether the response should use HTTP Strict Transport Security
        /// </summary>
        public bool UseHsts { get; set; }

        /// <summary>
        /// Indicates whether the response should use Public Key Pinning Extension for HTTP
        /// </summary>
        public bool UseHpkp { get; set; }

        /// <summary>
        /// Indicates whether the response should use X-Frame-Options
        /// </summary>
        public bool UseXFrameOptions { get; set; }

        /// <summary>
        /// Indicates whether the response should use X-XSS-Protection
        /// </summary>                
        public bool UseXssProtection { get; set; }

        /// <summary>
        /// Indicates whether the response should use X-Content-Type-Options
        /// </summary>
        public bool UseXContentTypeOptions { get; set; }

        /// <summary>
        /// The HTTP Strict Transport Security configuration to use
        /// </summary>
        public HstsConfiguration HstsConfiguration { get; set; }

        /// <summary>
        /// The Public Key Pinning Extension for HTTP configuration to use
        /// </summary>
        public HPKPConfiguration HpkpConfiguration { get; set; }

        /// <summary>
        /// The X-Frame-Options configuration to use
        /// </summary>
        public XFrameOptionsConfiguration XFrameOptionsConfiguration { get; set; }

        /// <summary>
        /// The X-XSS-Protection configuration to use
        /// </summary>
        public XssConfiguration XssConfiguration { get; set; }
        
        public SecureHeadersMiddlewareConfiguration()
        {
            UseHsts = true;
            UseHpkp = true;
            UseXFrameOptions = true;
            UseXssProtection = true;
            UseXContentTypeOptions = true;

            HstsConfiguration = new HstsConfiguration();
            HpkpConfiguration = new HPKPConfiguration();
            XFrameOptionsConfiguration = new XFrameOptionsConfiguration();
            XssConfiguration = new XssConfiguration();
        }
        
    }
}
