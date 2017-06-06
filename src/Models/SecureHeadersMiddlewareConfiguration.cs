namespace OwaspHeaders.Core.Models
{
    public class SecureHeadersMiddlewareConfiguration
    {
        /// <summary>
        /// Indicates whether the response should use HTTP Strict Transport Security
        /// </summary>
        public bool UseHsts { get; set; }
        /// <summary>
        /// The HTTP Strict Transport Security configuration to use
        /// </summary>
        public HstsConfiguration HstsConfiguration { get; set; }

        /// <summary>
        /// Indicates whether the response should use Public Key Pinning Extension for HTTP
        /// </summary>
        public bool UseHpkp { get; set; }

        /// <summary>
        /// The Public Key Pinning Extension for HTTP configuration to use
        /// </summary>
        public HPKPConfiguration HpkpConfiguration { get; set; }

        /// <summary>
        /// Indicates whether the response should use X Frame Options
        /// </summary>
        public bool UseXFrameOptions { get; set; }

        /// <summary>
        /// The X-Frame-Options configuration to use
        /// </summary>
        public XFrameOptionsConfiguration XFrameOptionsConfiguration { get; set; }

        /// <summary>
        /// Inidcates whether to use XSS-Protection options
        /// </summary>
        public bool UseXssProtection { get; set; }

        public XssConfiguration XssConfiguration { get; set; }
        
        public SecureHeadersMiddlewareConfiguration()
        {
            UseHsts = true;
            HstsConfiguration = new HstsConfiguration();

            UseHpkp = true;
            HpkpConfiguration = new HPKPConfiguration();

            UseXFrameOptions = true;
            XFrameOptionsConfiguration = new XFrameOptionsConfiguration();

            UseXssProtection = true;
            XssConfiguration = new XssConfiguration();
        }
        
    }
}
