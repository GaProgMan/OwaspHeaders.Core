namespace OwaspHeaders.Core.Models
{
    public interface ISecureHeadersMiddlewareConfiguration
    {
        bool UseHsts { get; set; }
        bool UseHpkp { get; set; }
        bool UseXFrameOptions { get; set; }
        bool UseXssProtection { get; set; }
        bool UseXContentTypeOptions { get; set; }
        bool UseContentSecurityPolicy { get; set; }

        HstsConfiguration HstsConfiguration { get; set; }
        HPKPConfiguration HpkpConfiguration { get; set; }
        XFrameOptionsConfiguration XFrameOptionsConfiguration { get; set; }
        XssConfiguration XssConfiguration { get; set; }
        ContentSecurityPolicyConfiguration ContentSecurityPolicyConfiguration { get;set; }
    }
}
