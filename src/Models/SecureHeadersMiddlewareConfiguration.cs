namespace OwaspHeaders.Core.Models;

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
    /// <para>Indicates whether the response should use a Report-Only version
    /// of the Content-Security-Policy</para>
    /// <para>This can be useful when in development mode, as the browser will
    /// not block content which violates the CSP rule set - it will report to
    /// the supplied ReportUri</para>
    /// </summary>
    public bool UseContentSecurityPolicyReportOnly { get; set; }

    /// <summary>
    /// Indicates whether the response should use X-Content-Security-Policy for
    /// Internet Explorer compatibility
    /// </summary>
    public bool UseXContentSecurityPolicy { get; set; }

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
    /// <remarks>
    /// The Expect-CT header has been deprecated by OWASP. The setter is marked
    /// <see cref="ObsoleteAttribute"/> so that callers who bypass the (also obsolete)
    /// <c>UseExpectCt</c> builder extension still see a deprecation warning. The getter
    /// remains non-obsolete so the middleware can read the flag without warnings.
    /// </remarks>
    public bool UseExpectCt
    {
        get;
        [Obsolete("Expect-CT has been deprecated by OWASP and will be removed in a future major version. See https://owasp.org/www-project-secure-headers/#expect-ct for details.", false)]
        set;
    }

    /// <summary>
    /// Indicates whether the response should use Cache-Control
    /// </summary>
    public bool UseCacheControl { get; set; }

    /// <summary>
    /// Indicates whether the response should use Cross-Origin-Resource-Policy
    /// </summary>
    public bool UseCrossOriginResourcePolicy { get; set; }

    /// <summary>
    /// Indicates whether the response should use Cross-Origin-Opener-Policy
    /// </summary>
    public bool UseCrossOriginOpenerPolicy { get; set; }

    /// <summary>
    /// Indicates whether the response should use Cross-Origin-Embedder-Policy
    /// </summary>
    public bool UseCrossOriginEmbedderPolicy { get; set; }

    /// <summary>
    /// Indicates whether the response should use the Reporting-Endpoints header
    /// </summary>
    public bool UseReportingEndPoints { get; set; }

    /// <summary>
    /// Indicates whether the response should use Clear-Site-Data
    /// </summary>
    public bool UseClearSiteData { get; set; }

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
    /// The Content-Security-Policy-Report-Only configuration to use 
    /// </summary>
    public ContentSecurityPolicyReportOnlyConfiguration ContentSecurityPolicyReportOnlyConfiguration { get; set; }

    /// <summary>
    /// The X-Permitted-Cross-Domain-Policy configuration to use
    /// </summary>
    public PermittedCrossDomainPolicyConfiguration PermittedCrossDomainPolicyConfiguration { get; set; }

    /// <summary>
    /// The Referrer-Policy configuration to use
    /// </summary>
    public ReferrerPolicy ReferrerPolicy { get; set; }

    /// <summary>
    /// The Cache-Control configuration to use
    /// </summary>
    public CacheControl CacheControl { get; set; }

    /// <summary>
    /// The Expect-CT configuration to use
    /// </summary>
    /// <remarks>
    /// The Expect-CT header has been deprecated by OWASP. The setter is marked
    /// <see cref="ObsoleteAttribute"/> so that callers who assign this directly still see
    /// a deprecation warning. The getter remains non-obsolete so the middleware can read
    /// the value without warnings.
    /// </remarks>
    public ExpectCt ExpectCt
    {
        get;
        [Obsolete("Expect-CT has been deprecated by OWASP and will be removed in a future major version. See https://owasp.org/www-project-secure-headers/#expect-ct for details.", false)]
        set;
    }

    public CrossOriginResourcePolicy CrossOriginResourcePolicy { get; set; }

    public CrossOriginOpenerPolicy CrossOriginOpenerPolicy { get; set; }

    public CrossOriginEmbedderPolicy CrossOriginEmbedderPolicy { get; set; }

    public ReportingEndpointsPolicy ReportingEndpointsPolicy { get; set; }

    /// <summary>
    /// The Clear-Site-Data path configuration to use
    /// </summary>
    public ClearSiteDataPathConfiguration ClearSiteDataPathConfiguration { get; set; }

    /// <summary>
    /// A list of URLs that, when requested, should be ignored completely by
    /// the middleware
    /// </summary>
    public List<string> UrlsToIgnore { get; set; } = [];

    /// <summary>
    /// Configuration for logging event IDs. Allows customization to avoid conflicts
    /// with application event IDs. Defaults to standard SecureHeaders event ID ranges.
    /// </summary>
    public SecureHeadersLoggingConfiguration LoggingConfiguration { get; set; } = new();

    /// <summary>
    /// Returns a list of validation issues describing every <c>UseX</c> flag that has
    /// been set to <c>true</c> without a matching configuration object being populated.
    /// An empty list indicates the configuration is internally consistent.
    /// </summary>
    /// <remarks>
    /// Mismatches typically occur when a flag is set directly on the configuration
    /// instance instead of via the corresponding builder extension on
    /// <see cref="SecureHeadersMiddlewareBuilder"/>, which would normally allocate both
    /// the flag and the matching configuration object together.
    /// </remarks>
    public IReadOnlyList<string> Validate()
    {
        var issues = new List<string>();

        void Check(bool enabled, object configuration, string flagName)
        {
            if (enabled && configuration is null)
            {
                issues.Add(
                    $"{flagName} is true but its matching configuration object is null. " +
                    $"Configure this header via the corresponding SecureHeadersMiddlewareBuilder extension method " +
                    $"instead of setting {flagName} directly.");
            }
        }

        // UseXContentTypeOptions is intentionally omitted: the X-Content-Type-Options header
        // emits a constant value ("nosniff") and has no backing configuration object, so there
        // is nothing to validate against.
        Check(UseHsts, HstsConfiguration, nameof(UseHsts));
        Check(UseXFrameOptions, XFrameOptionsConfiguration, nameof(UseXFrameOptions));
        Check(UseXssProtection, XssConfiguration, nameof(UseXssProtection));
        Check(UseContentSecurityPolicy, ContentSecurityPolicyConfiguration, nameof(UseContentSecurityPolicy));
        Check(UseContentSecurityPolicyReportOnly, ContentSecurityPolicyReportOnlyConfiguration,
            nameof(UseContentSecurityPolicyReportOnly));
        Check(UseXContentSecurityPolicy, ContentSecurityPolicyConfiguration, nameof(UseXContentSecurityPolicy));
        Check(UsePermittedCrossDomainPolicy, PermittedCrossDomainPolicyConfiguration,
            nameof(UsePermittedCrossDomainPolicy));
        Check(UseReferrerPolicy, ReferrerPolicy, nameof(UseReferrerPolicy));
        Check(UseExpectCt, ExpectCt, nameof(UseExpectCt));
        Check(UseCacheControl, CacheControl, nameof(UseCacheControl));
        Check(UseCrossOriginResourcePolicy, CrossOriginResourcePolicy, nameof(UseCrossOriginResourcePolicy));
        Check(UseCrossOriginOpenerPolicy, CrossOriginOpenerPolicy, nameof(UseCrossOriginOpenerPolicy));
        Check(UseCrossOriginEmbedderPolicy, CrossOriginEmbedderPolicy, nameof(UseCrossOriginEmbedderPolicy));
        Check(UseReportingEndPoints, ReportingEndpointsPolicy, nameof(UseReportingEndPoints));
        Check(UseClearSiteData, ClearSiteDataPathConfiguration, nameof(UseClearSiteData));

        return issues;
    }
}
