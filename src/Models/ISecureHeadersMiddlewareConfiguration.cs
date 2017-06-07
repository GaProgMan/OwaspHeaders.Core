namespace OwaspHeaders.Core.Models
{
    public interface ISecureHeadersMiddlewareConfiguration
    {
        bool UseHsts { get; set; }
        HstsConfiguration HstsConfiguration { get; set; }
        bool UseHpkp { get; set; }
        HPKPConfiguration HpkpConfiguration { get; set; }
        bool UseXFrameOptions { get; set; }
        XFrameOptionsConfiguration XFrameOptionsConfiguration { get; set; }
        bool UseXssProtection { get; set; }
        XssConfiguration XssConfiguration { get; set; }
    }
}
