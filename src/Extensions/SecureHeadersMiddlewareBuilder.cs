using static OwaspHeaders.Core.Models.CrossOriginResourcePolicy;

namespace OwaspHeaders.Core.Extensions;

/// <summary>
/// The original fluent configuration surface, in which every method is an extension method on
/// <see cref="SecureHeadersMiddlewareConfiguration"/> itself.
/// </summary>
/// <remarks>
/// <para>
/// Every method here now forwards to the equivalent instance method on
/// <see cref="SecureHeadersBuilder"/>, which owns the real implementations. The forwarders wrap
/// the caller's configuration rather than allocating a new one, so they keep mutating the
/// instance they are given and returning that same reference, exactly as before.
/// </para>
/// <para>
/// Prefer configuring the middleware in place:
/// <code>app.UseSecureHeadersMiddleware(opt => opt.UseRecommendedDefaults());</code>
/// </para>
/// </remarks>
public static class SecureHeadersMiddlewareBuilder
{
    /// <summary>
    /// Creates a new, empty <see cref="SecureHeadersMiddlewareConfiguration"/> to configure.
    /// </summary>
    public static SecureHeadersMiddlewareConfiguration CreateBuilder()
    {
        return new SecureHeadersBuilder().Build();
    }

    /// <inheritdoc cref="SecureHeadersBuilder.UseHsts"/>
    public static SecureHeadersMiddlewareConfiguration UseHsts
    (this SecureHeadersMiddlewareConfiguration config,
        int maxAge = 31536000, bool includeSubDomains = true)
        => new SecureHeadersBuilder(config).UseHsts(maxAge, includeSubDomains).Build();

    /// <inheritdoc cref="SecureHeadersBuilder.UseXFrameOptions"/>
    public static SecureHeadersMiddlewareConfiguration UseXFrameOptions
    (this SecureHeadersMiddlewareConfiguration config,
        XFrameOptions xFrameOption = XFrameOptions.Deny,
        string domain = null)
        => new SecureHeadersBuilder(config).UseXFrameOptions(xFrameOption, domain).Build();

    /// <inheritdoc cref="SecureHeadersBuilder.UseXssProtection"/>
    public static SecureHeadersMiddlewareConfiguration UseXssProtection
        (this SecureHeadersMiddlewareConfiguration config)
        => new SecureHeadersBuilder(config).UseXssProtection().Build();

    /// <inheritdoc cref="SecureHeadersBuilder.UseContentTypeOptions"/>
    public static SecureHeadersMiddlewareConfiguration UseContentTypeOptions
        (this SecureHeadersMiddlewareConfiguration config)
        => new SecureHeadersBuilder(config).UseContentTypeOptions().Build();

    /// <inheritdoc cref="SecureHeadersBuilder.UseDefaultContentSecurityPolicy"/>
    public static SecureHeadersMiddlewareConfiguration UseDefaultContentSecurityPolicy
        (this SecureHeadersMiddlewareConfiguration config)
        => new SecureHeadersBuilder(config).UseDefaultContentSecurityPolicy().Build();

    /// <inheritdoc cref="SecureHeadersBuilder.UseContentSecurityPolicy"/>
    public static SecureHeadersMiddlewareConfiguration UseContentSecurityPolicy
    (this SecureHeadersMiddlewareConfiguration config,
        string pluginTypes = null, bool blockAllMixedContent = true,
        bool upgradeInsecureRequests = true, string referrer = null,
        string reportUri = null, bool useXContentSecurityPolicy = false, string reportTo = null)
        => new SecureHeadersBuilder(config).UseContentSecurityPolicy(pluginTypes, blockAllMixedContent,
            upgradeInsecureRequests, referrer, reportUri, useXContentSecurityPolicy, reportTo).Build();

    /// <inheritdoc cref="SecureHeadersBuilder.UseContentSecurityPolicyReportUriOnly"/>
    public static SecureHeadersMiddlewareConfiguration UseContentSecurityPolicyReportUriOnly
    (this SecureHeadersMiddlewareConfiguration config, string reportUri,
        string pluginTypes = null, bool blockAllMixedContent = true,
        bool upgradeInsecureRequests = true, string referrer = null,
        bool useXContentSecurityPolicy = false, string reportTo = null)
        => new SecureHeadersBuilder(config).UseContentSecurityPolicyReportUriOnly(reportUri, pluginTypes,
            blockAllMixedContent, upgradeInsecureRequests, referrer, useXContentSecurityPolicy, reportTo).Build();

    /// <summary>
    /// Configures Content Security Policy Report Only mode.
    /// </summary>
    /// <remarks>
    /// This method has been renamed to UseContentSecurityPolicyReportUriOnly for clarity.
    /// Please update your code to use the new method name.
    /// </remarks>
    [Obsolete("UseContentSecurityPolicyReportOnly has been renamed to UseContentSecurityPolicyReportUriOnly. Please use the new method name.", false)]
    public static SecureHeadersMiddlewareConfiguration UseContentSecurityPolicyReportOnly
    (this SecureHeadersMiddlewareConfiguration config, string reportUri,
        string pluginTypes = null, bool blockAllMixedContent = true,
        bool upgradeInsecureRequests = true, string referrer = null,
        bool useXContentSecurityPolicy = false, string reportTo = null)
        => config.UseContentSecurityPolicyReportUriOnly(reportUri, pluginTypes, blockAllMixedContent,
            upgradeInsecureRequests, referrer, useXContentSecurityPolicy, reportTo);

    /// <inheritdoc cref="SecureHeadersBuilder.UsePermittedCrossDomainPolicies"/>
    public static SecureHeadersMiddlewareConfiguration UsePermittedCrossDomainPolicies
    (this SecureHeadersMiddlewareConfiguration config,
        XPermittedCrossDomainOptionValue xPermittedCrossDomainOptionValue =
            XPermittedCrossDomainOptionValue.none)
        => new SecureHeadersBuilder(config)
            .UsePermittedCrossDomainPolicies(xPermittedCrossDomainOptionValue).Build();

    /// <inheritdoc cref="SecureHeadersBuilder.UseReferrerPolicy"/>
    public static SecureHeadersMiddlewareConfiguration UseReferrerPolicy
    (this SecureHeadersMiddlewareConfiguration config,
        ReferrerPolicyOptions referrerPolicyOption = ReferrerPolicyOptions.noReferrer)
        => new SecureHeadersBuilder(config).UseReferrerPolicy(referrerPolicyOption).Build();

    /// <inheritdoc cref="SecureHeadersBuilder.UseCacheControl"/>
    public static SecureHeadersMiddlewareConfiguration UseCacheControl
    (this SecureHeadersMiddlewareConfiguration config,
        bool @private = false, int maxAge = 0, bool noCache = false, bool noStore = true,
        bool mustRevalidate = false)
        => new SecureHeadersBuilder(config)
            .UseCacheControl(@private, maxAge, noCache, noStore, mustRevalidate).Build();

    /// <inheritdoc cref="SecureHeadersBuilder.UseCrossOriginResourcePolicy"/>
    public static SecureHeadersMiddlewareConfiguration UseCrossOriginResourcePolicy(
        this SecureHeadersMiddlewareConfiguration config,
        CrossOriginResourceOptions value = CrossOriginResourceOptions.SameOrigin)
        => new SecureHeadersBuilder(config).UseCrossOriginResourcePolicy(value).Build();

    /// <inheritdoc cref="SecureHeadersBuilder.UseCrossOriginOpenerPolicy"/>
    public static SecureHeadersMiddlewareConfiguration UseCrossOriginOpenerPolicy(
        this SecureHeadersMiddlewareConfiguration config,
        CrossOriginOpenerPolicy.CrossOriginOpenerOptions value =
            CrossOriginOpenerPolicy.CrossOriginOpenerOptions.SameOrigin)
        => new SecureHeadersBuilder(config).UseCrossOriginOpenerPolicy(value).Build();

    /// <inheritdoc cref="SecureHeadersBuilder.UseCrossOriginEmbedderPolicy"/>
    public static SecureHeadersMiddlewareConfiguration UseCrossOriginEmbedderPolicy(
        this SecureHeadersMiddlewareConfiguration config,
        CrossOriginEmbedderPolicy.CrossOriginEmbedderOptions value =
            CrossOriginEmbedderPolicy.CrossOriginEmbedderOptions.RequireCorp)
        => new SecureHeadersBuilder(config).UseCrossOriginEmbedderPolicy(value).Build();

    /// <inheritdoc cref="SecureHeadersBuilder.UseReportingEndpointsPolicy"/>
    public static SecureHeadersMiddlewareConfiguration UseReportingEndpointsPolicy(
        this SecureHeadersMiddlewareConfiguration config,
        Dictionary<string, Uri> endpoints)
        => new SecureHeadersBuilder(config).UseReportingEndpointsPolicy(endpoints).Build();

    /// <inheritdoc cref="SecureHeadersBuilder.SetUrlsToIgnore"/>
    public static SecureHeadersMiddlewareConfiguration SetUrlsToIgnore(
        this SecureHeadersMiddlewareConfiguration config,
        List<string> urlsToIgnore = null)
        => new SecureHeadersBuilder(config).SetUrlsToIgnore(urlsToIgnore).Build();

    /// <inheritdoc cref="SecureHeadersBuilder.WithLoggingEventIds"/>
    public static SecureHeadersMiddlewareConfiguration WithLoggingEventIds(
        this SecureHeadersMiddlewareConfiguration config,
        SecureHeadersLoggingConfiguration loggingConfig)
        => new SecureHeadersBuilder(config).WithLoggingEventIds(loggingConfig).Build();

    /// <inheritdoc cref="SecureHeadersBuilder.WithLoggingEventIdBase"/>
    public static SecureHeadersMiddlewareConfiguration WithLoggingEventIdBase(
        this SecureHeadersMiddlewareConfiguration config,
        int baseEventId)
        => new SecureHeadersBuilder(config).WithLoggingEventIdBase(baseEventId).Build();

    /// <inheritdoc cref="SecureHeadersBuilder.UseClearSiteData"/>
    public static SecureHeadersMiddlewareConfiguration UseClearSiteData(
        this SecureHeadersMiddlewareConfiguration config,
        params ClearSiteDataOptions[] directiveOptions)
        => new SecureHeadersBuilder(config).UseClearSiteData(directiveOptions).Build();

    /// <inheritdoc cref="SecureHeadersBuilder.UseClearSiteDataForPaths"/>
    public static SecureHeadersMiddlewareConfiguration UseClearSiteDataForPaths(
        this SecureHeadersMiddlewareConfiguration config,
        Dictionary<string, ClearSiteDataOptions[]> pathConfigurations,
        ClearSiteDataOptions[] defaultConfiguration = null)
        => new SecureHeadersBuilder(config)
            .UseClearSiteDataForPaths(pathConfigurations, defaultConfiguration).Build();

    /// <inheritdoc cref="SecureHeadersBuilder.AddClearSiteDataPath"/>
    public static SecureHeadersMiddlewareConfiguration AddClearSiteDataPath(
        this SecureHeadersMiddlewareConfiguration config,
        string path,
        params ClearSiteDataOptions[] directiveOptions)
        => new SecureHeadersBuilder(config).AddClearSiteDataPath(path, directiveOptions).Build();

    /// <summary>
    /// Return the completed <see cref="SecureHeadersMiddlewareConfiguration"/> ready for consumption by the
    /// <see cref="SecureHeadersMiddleware"/> class
    /// </summary>
    public static SecureHeadersMiddlewareConfiguration Build
        (this SecureHeadersMiddlewareConfiguration config)
    {
        return config;
    }
}
