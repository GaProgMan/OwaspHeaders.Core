namespace OwaspHeaders.Core.Models
{
    public class SecureHeadersMiddlewareConfiguration
    {
        /// <summary>
        /// Indicates whether the response should use HTTP Strict Transport Security
        /// </summary>
        public bool UseHsts { get; set; }

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
        /// Indicates whether the response should use Content-Security-Policy
        /// </summary>
        public bool UseContentSecurityPolicy { get; set; }

        /// <summary>
        /// Indicates whether the response should use X-Permitted-Cross-Domain-Policy
        /// </summary>
        public bool UsePermittedCrossDomainPolicy { get; set; }

        /// <summary>
        /// Indicates whether the response should use Referrer-Policy
        /// </summary>
        public bool UseReferrerPolicy { get; set; }
        
        /// <summary>
        /// Indicates whether the response should use Expect-CT
        /// </summary>
        public bool UseExpectCt { get; set; }
        
        public bool RemoveXPoweredByHeader { get; set; }

        /// <summary>
        /// The HTTP Strict Transport Security configuration to use
        /// </summary>
        public HstsConfiguration HstsConfiguration { get; set; }

        /// <summary>
        /// The X-Frame-Options configuration to use
        /// </summary>
        public XFrameOptionsConfiguration XFrameOptionsConfiguration { get; set; }

        /// <summary>
        /// The X-XSS-Protection configuration to use
        /// </summary>
        public XssConfiguration XssConfiguration { get; set; }

        /// <summary>
        /// The Content-Security-Policy configuration to use
        /// </summary>
        public ContentSecurityPolicyConfiguration ContentSecurityPolicyConfiguration { get; set; }

        /// <summary>
        /// The X-Permitted-Cross-Domain-Policy configuration to use
        /// </summary>
        public PermittedCrossDomainPolicyConfiguration PermittedCrossDomainPolicyConfiguration { get; set; }

        /// <summary>
        /// The Referrer-Policy configuration to use
        /// </summary>
        public ReferrerPolicy ReferrerPolicy { get; set; }
        
        /// <summary>
        /// The Expect-CT configuration to use
        /// </summary>
        public ExpectCt ExpectCt { get; set; }
    }
}
