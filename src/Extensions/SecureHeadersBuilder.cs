// Note:  some commments (especially those which explain what the different
//        parameters for each header) are taken from the OWASP Secure Headers
//        page. The original comments can be found at:
//                https://www.owasp.org/index.php/OWASP_Secure_Headers_Project
using static OwaspHeaders.Core.Models.CrossOriginResourcePolicy;

namespace OwaspHeaders.Core.Extensions;

/// <summary>
/// Fluent builder for a <see cref="SecureHeadersMiddlewareConfiguration"/>.
/// </summary>
/// <remarks>
/// <para>
/// The supported way to configure the middleware is to let it hand you a builder:
/// <code>app.UseSecureHeadersMiddleware(opt => opt.UseRecommendedDefaults());</code>
/// The configure delegate is invoked exactly once, while the request pipeline is being
/// built, rather than once per request.
/// </para>
/// <para>
/// A new builder starts empty: it does not pre-apply the OWASP recommended headers. Call
/// <see cref="UseRecommendedDefaults"/> to start from that set.
/// </para>
/// <para>
/// Note for maintainers: every method here writes to the properties of the wrapped
/// configuration directly. Do not call the <see cref="SecureHeadersMiddlewareBuilder"/>
/// extension methods from inside this class. Those resolve only because member lookup finds
/// no applicable *method* named (for example) <c>UseHsts</c> on the configuration and falls
/// back to extension-method lookup past the <c>UseHsts</c> *property*. They are deprecated
/// and disappear in version 12, at which point such a call would become CS1955.
/// </para>
/// </remarks>
public sealed class SecureHeadersBuilder
{
    private readonly SecureHeadersMiddlewareConfiguration _configuration;

    /// <summary>
    /// Creates a builder over a new, empty <see cref="SecureHeadersMiddlewareConfiguration"/>.
    /// </summary>
    public SecureHeadersBuilder() : this(new SecureHeadersMiddlewareConfiguration())
    {
    }

    /// <summary>
    /// Wraps an existing configuration instance rather than allocating a new one.
    /// </summary>
    /// <remarks>
    /// Used by the deprecated <see cref="SecureHeadersMiddlewareBuilder"/> extension methods,
    /// which mutate the caller's instance in place and must keep doing so.
    /// </remarks>
    internal SecureHeadersBuilder(SecureHeadersMiddlewareConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Returns the configured <see cref="SecureHeadersMiddlewareConfiguration"/>, ready for
    /// consumption by the <see cref="SecureHeadersMiddleware"/> class.
    /// </summary>
    public SecureHeadersMiddlewareConfiguration Build()
    {
        return _configuration;
    }

    /// <summary>
    /// Builds the configuration described by <paramref name="configure"/> and throws if it is
    /// not valid.
    /// </summary>
    /// <remarks>
    /// This takes the same delegate as
    /// <c>UseSecureHeadersMiddleware(Action&lt;SecureHeadersBuilder&gt;)</c>, so a consumer can
    /// hoist their configuration into a named method and assert its validity from a test without
    /// standing up a host:
    /// <code>
    /// static void ConfigureSecureHeaders(SecureHeadersBuilder opt) => opt.UseRecommendedDefaults();
    ///
    /// // in Program.cs
    /// app.UseSecureHeadersMiddleware(ConfigureSecureHeaders);
    ///
    /// // in a test
    /// SecureHeadersBuilder.BuildAndValidate(ConfigureSecureHeaders);
    /// </code>
    /// </remarks>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="configure"/> is null.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when the resulting configuration is not internally consistent.
    /// </exception>
    public static SecureHeadersMiddlewareConfiguration BuildAndValidate(
        Action<SecureHeadersBuilder> configure)
    {
        ObjectGuardClauses.ObjectCannotBeNull(configure, nameof(configure),
            "cannot be null when configuring OWASP Secure Headers in OwaspHeaders.Core");

        var builder = new SecureHeadersBuilder();
        configure(builder);

        var configuration = builder.Build();
        configuration.ValidateOrThrow();

        return configuration;
    }

    /// <summary>
    /// Applies the full set of headers recommended by the OWASP Secure Headers Project,
    /// using the default values described on each of the <c>Use...</c> methods.
    /// </summary>
    /// <remarks>
    /// This is the starting point most consumers want. Call it first, then override
    /// individual headers:
    /// <code>
    /// app.UseSecureHeadersMiddleware(opt =>
    /// {
    ///     opt.UseRecommendedDefaults();
    ///     opt.UseHsts(maxAge: 63072000);
    /// });
    /// </code>
    /// Please see the following url for the current best practises:
    /// https://www.owasp.org/index.php/OWASP_Secure_Headers_Project#tab=Best_Practices
    /// </remarks>
    public SecureHeadersBuilder UseRecommendedDefaults()
    {
        // When the OWASP Secure Headers project recommends the use of the Reporting-Endpoints
        // header, we will enable it here.
        return UseHsts()
            .UseXFrameOptions()
            .UseContentTypeOptions()
            .UseDefaultContentSecurityPolicy()
            .UsePermittedCrossDomainPolicies()
            .UseReferrerPolicy()
            .UseCacheControl()
            .UseXssProtection()
            .UseCrossOriginResourcePolicy()
            .UseCrossOriginOpenerPolicy()
            .UseCrossOriginEmbedderPolicy();
    }

    /// <summary>
    /// Includes the HTTP Strict Transport Security header in all responses
    /// generated by the application which consumes this middleware
    /// </summary>
    /// <param name="maxAge">
    /// The The time, in seconds, that the browser should remember that this
    /// site is only to be accessed using HTTPS
    /// </param>
    /// <param name="includeSubDomains">
    /// If this optional parameter is specified, this rule applies to all of
    /// the site's subdomains as well
    /// </param>
    /// <remarks>
    /// If no values for <param name="maxAge"/> or <param name="includeSubDomains"/>
    /// are provided, then default ones will be used. These default values will be
    /// based on the OWASP best practises values for HSTS.
    /// </remarks>
    public SecureHeadersBuilder UseHsts(int maxAge = 31536000, bool includeSubDomains = true)
    {
        _configuration.UseHsts = true;
        _configuration.HstsConfiguration = new HstsConfiguration(maxAge, includeSubDomains);

        return this;
    }

    /// <summary>
    /// Declares a policy communicated from a host to the client browser on whether
    /// the browser must not display the transmitted content in frames of other web pages
    /// </summary>
    /// <param name="xFrameOption">
    /// Whether or not we should allow rendering this site within a frame.
    /// Applicable values are: deny; sameorigin; and allowfrom
    /// </param>
    /// <param name="domain">
    /// If allowfrom is supplied, this optional parameter describes the domain in which
    /// our site is permitted to be loaded within a frame
    /// </param>
    /// <remarks>
    /// If no value for <param name="xFrameOption"/> is rovided, then default one will be
    /// used. This default value is based on the OWASP best practises value for X-Frame-Options.
    /// </remarks>
    public SecureHeadersBuilder UseXFrameOptions(
        XFrameOptions xFrameOption = XFrameOptions.Deny,
        string domain = null)
    {
        _configuration.UseXFrameOptions = true;
        _configuration.XFrameOptionsConfiguration = new XFrameOptionsConfiguration(xFrameOption, domain);

        return this;
    }

    /// <summary>
    /// Enables the Cross Site Scripting protection filter in the client browser.
    /// </summary>
    /// <remarks>
    /// This overload will use the X-XSS-Protection value of 0. This effectively disables
    /// the XSS Auditor, and is required for modern browsers. Please ensure that you have
    /// a value Content-Security Policy enabled, otherwise you are opening yourself up to
    /// a world of trouble.
    /// The XSS Auditor needs to be disabled because it can lead to client-side security
    /// issues in modern browsers.
    /// </remarks>
    public SecureHeadersBuilder UseXssProtection()
    {
        _configuration.UseXssProtection = true;
        _configuration.XssConfiguration = new XssConfiguration();

        return this;
    }

    /// <summary>
    /// Setting this header will prevent the browser from interpreting files as something
    /// else than declared by the content type in the HTTP headers
    /// </summary>
    /// <remarks>
    /// There is no value to pass in here, OWASP recommends that if you use this header
    /// (X-ContentType-Options), then the value of "nosniff" be used. "nosniff" is the default
    /// value for this header when using this middleware class.
    /// </remarks>
    public SecureHeadersBuilder UseContentTypeOptions()
    {
        _configuration.UseXContentTypeOptions = true;

        return this;
    }

    /// <summary>
    /// CSP prevents a wide range of attacks, including Cross-site scripting and other
    /// cross-site injections.
    /// </summary>
    /// <remarks>
    /// This method sets up a CSP header with:
    ///  - all mixed content blocked
    ///  - all insecure
    ///  - requests upgraded to HTTPS
    ///  - a ScriptSrc of "self"
    ///  - an ObjectSrc of "self"
    /// </remarks>
    public SecureHeadersBuilder UseDefaultContentSecurityPolicy()
    {
        _configuration.UseContentSecurityPolicy = true;

        _configuration.ContentSecurityPolicyConfiguration = new ContentSecurityPolicyConfiguration
            (null, true, true, null, null, null);

        // Written to this policy directly rather than through SetCspUris, which also applies to a
        // report-only policy if one has already been configured.
        _configuration.ContentSecurityPolicyConfiguration.SetCspUri(
            [ContentSecurityPolicyHelpers.CreateSelfDirective()],
            CspUriType.Script);

        _configuration.ContentSecurityPolicyConfiguration.SetCspUri(
            [ContentSecurityPolicyHelpers.CreateSelfDirective()],
            CspUriType.Object);

        return this;
    }

    /// <summary>
    /// CSP prevents a wide range of attacks, including Cross-site scripting and other
    /// cross-site injections.
    /// </summary>
    /// <param name="pluginTypes">
    /// The set of plugins that can be invoked by the protected resource by limiting the
    /// types of resources that can be embedded
    /// </param>
    /// <param name="blockAllMixedContent">
    /// Prevent user agent from loading mixed content.
    /// </param>
    /// <param name="upgradeInsecureRequests">
    /// Instructs user agent to download insecure resources using HTTPS.
    /// </param>
    /// <param name="reportUri">
    /// Specifies a URI to which the user agent sends reports about policy violation.
    /// </param>
    /// <param name="useXContentSecurityPolicy">
    /// Specifies if we should use X-Content-Security-Policy header as well for compatibility with Internet Explorer.
    /// </param>
    /// <param name="reportTo">
    /// The name of the endpoint that we want to report any CSP violations to.
    /// </param>
    /// <remarks>
    /// Requires consumer to set up their own Content Security Policy Rules via calls to
    /// <see cref="SetCspUris"/>
    /// </remarks>
    public SecureHeadersBuilder UseContentSecurityPolicy(
        string pluginTypes = null, bool blockAllMixedContent = true,
        bool upgradeInsecureRequests = true, string referrer = null,
        string reportUri = null, bool useXContentSecurityPolicy = false, string reportTo = null)
    {
        _configuration.UseContentSecurityPolicy = true;
        _configuration.UseXContentSecurityPolicy = useXContentSecurityPolicy;
        _configuration.ContentSecurityPolicyConfiguration = new ContentSecurityPolicyConfiguration
            (pluginTypes, blockAllMixedContent, upgradeInsecureRequests, referrer, reportUri, reportTo);

        return this;
    }

    /// <summary>
    /// Configures Content Security Policy Report Only mode, reporting to the supplied report URI.
    /// </summary>
    /// <param name="useXContentSecurityPolicy">
    /// Must be <c>false</c>. X-Content-Security-Policy is an enforcing header with no report-only
    /// form, so it cannot be enabled from here: call <see cref="UseContentSecurityPolicy"/> with
    /// <c>useXContentSecurityPolicy: true</c> instead. The parameter remains only so that this
    /// method's signature is unchanged, and will be removed in version 12.
    /// </param>
    /// <remarks>
    /// Add directives to the report-only policy by calling <see cref="SetCspUris"/> and
    /// <see cref="SetCspSandBox"/> after this method.
    /// </remarks>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="useXContentSecurityPolicy"/> is <c>true</c>.
    /// </exception>
    public SecureHeadersBuilder UseContentSecurityPolicyReportUriOnly(
        string reportUri,
        string pluginTypes = null, bool blockAllMixedContent = true,
        bool upgradeInsecureRequests = true, string referrer = null,
        bool useXContentSecurityPolicy = false, string reportTo = null)
    {
        // Report-only mode must neither enable nor disable X-Content-Security-Policy. Enabling it
        // here set the flag with no enforcing policy behind it, and the default of false switched
        // off an X-Content-Security-Policy that UseContentSecurityPolicy had enabled (issue #240).
        if (useXContentSecurityPolicy)
        {
            throw new ArgumentException(
                "X-Content-Security-Policy has no report-only form, so it cannot be enabled from a " +
                "report-only Content-Security-Policy method. Call " +
                "UseContentSecurityPolicy(useXContentSecurityPolicy: true) to emit it.",
                nameof(useXContentSecurityPolicy));
        }

        _configuration.UseContentSecurityPolicyReportOnly = true;

        _configuration.ContentSecurityPolicyReportOnlyConfiguration = new ContentSecurityPolicyReportOnlyConfiguration
            (pluginTypes, blockAllMixedContent, upgradeInsecureRequests, referrer, reportUri, reportTo);

        return this;
    }

    /// <summary>
    /// Configures Content Security Policy Report Only mode.
    /// </summary>
    /// <remarks>
    /// This method has been renamed to UseContentSecurityPolicyReportUriOnly for clarity.
    /// Please update your code to use the new method name.
    /// </remarks>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="useXContentSecurityPolicy"/> is <c>true</c>.
    /// </exception>
    [Obsolete("UseContentSecurityPolicyReportOnly has been renamed to UseContentSecurityPolicyReportUriOnly. Please use the new method name.", false)]
    public SecureHeadersBuilder UseContentSecurityPolicyReportOnly(
        string reportUri,
        string pluginTypes = null, bool blockAllMixedContent = true,
        bool upgradeInsecureRequests = true, string referrer = null,
        bool useXContentSecurityPolicy = false, string reportTo = null)
    {
        return UseContentSecurityPolicyReportUriOnly(reportUri, pluginTypes, blockAllMixedContent,
            upgradeInsecureRequests, referrer, useXContentSecurityPolicy, reportTo);
    }

    /// <summary>
    /// A cross-domain policy grants a web client permission to handle data across domains
    /// </summary>
    /// <remarks>
    /// If a <see cref="XPermittedCrossDomainOptionValue"/> is not supplied, then the default value of "none" will
    /// be used
    /// </remarks>
    public SecureHeadersBuilder UsePermittedCrossDomainPolicies(
        XPermittedCrossDomainOptionValue xPermittedCrossDomainOptionValue =
            XPermittedCrossDomainOptionValue.none)
    {
        _configuration.UsePermittedCrossDomainPolicy = true;

        _configuration.PermittedCrossDomainPolicyConfiguration =
            new PermittedCrossDomainPolicyConfiguration(xPermittedCrossDomainOptionValue);

        return this;
    }

    /// <summary>
    /// Governs which referrer information, sent in the Referer header, should be included with requests made
    /// </summary>
    /// <remarks>
    /// If a <see cref="ReferrerPolicyOptions"/> value is not supplied, then the default value of "no-referrer"
    /// will be used.
    /// </remarks>
    public SecureHeadersBuilder UseReferrerPolicy(
        ReferrerPolicyOptions referrerPolicyOption = ReferrerPolicyOptions.noReferrer)
    {
        _configuration.UseReferrerPolicy = true;

        _configuration.ReferrerPolicy = new ReferrerPolicy(referrerPolicyOption);

        return this;
    }

    /// <summary>
    /// The server did not return or returned an invalid 'Cache-Control' header which means page
    /// containing sensitive information (password, credit card, personal data, social security
    /// number, etc) could be stored on client side disk and then be exposed to unauthorised persons.
    /// This URL is flagged as a specific example.
    /// </summary>
    /// <param name="private">
    /// [OPTIONAL]
    /// Whether all or part of the HTTP response message is intended for a single user and must
    /// not be cached by a shared cache.
    /// </param>
    /// <param name="maxAge">
    /// [OPTIONAL]
    /// The maximum age, specified in seconds, that the HTTP client is willing to accept a response.
    /// </param>
    /// <exception cref="ArgumentException">
    /// An ArgumentException is thrown when no Report URI is supplied
    /// </exception>
    public SecureHeadersBuilder UseCacheControl(
        bool @private = false, int maxAge = 0, bool noCache = false, bool noStore = true,
        bool mustRevalidate = false)
    {
        _configuration.UseCacheControl = true;
        _configuration.CacheControl = new CacheControl(@private, maxAge, noCache, noStore, mustRevalidate);

        return this;
    }

    /// <summary>
    /// The HTTP Cross-Origin-Resource-Policy response header conveys a desire that the browser
    /// blocks no-cors cross-origin/cross-site requests to the given resource.
    /// </summary>
    /// <param name="value">
    /// The HTTP Cross-Origin-Resource-Policy response header value.
    /// </param>
    /// <remarks>
    /// Defaults to "same-origin" (<see cref="CrossOriginResourceOptions.SameOrigin"/>) which means
    /// that "Only requests from the same Origin (i.e. scheme + host + port) can read the resource."
    ///</remarks>
    public SecureHeadersBuilder UseCrossOriginResourcePolicy(
        CrossOriginResourceOptions value = CrossOriginResourceOptions.SameOrigin)
    {
        _configuration.UseCrossOriginResourcePolicy = true;
        _configuration.CrossOriginResourcePolicy = new CrossOriginResourcePolicy(value);

        return this;
    }

    /// <summary>
    /// The HTTP Cross-Origin-Opener-Policy (COOP) response header allows a website to control
    /// whether a new top-level document, opened using Window.open() or by navigating to a new
    /// page, is opened in the same browsing context group (BCG) or in a new browsing context group.
    /// Source: https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Cross-Origin-Opener-Policy
    /// </summary>
    /// <param name="value">
    /// The HTTP Cross-Origin-Opener-Policy response header value.
    /// </param>
    /// <remarks>
    /// Defaults to "same-origin" (<see cref="CrossOriginOpenerPolicy.CrossOriginOpenerOptions.SameOrigin"/>)
    /// which means that "Only requests from the same Origin (i.e. scheme + host + port) can read the resource."
    ///</remarks>
    public SecureHeadersBuilder UseCrossOriginOpenerPolicy(
        CrossOriginOpenerPolicy.CrossOriginOpenerOptions value =
            CrossOriginOpenerPolicy.CrossOriginOpenerOptions.SameOrigin)
    {
        _configuration.UseCrossOriginOpenerPolicy = true;
        _configuration.CrossOriginOpenerPolicy = new CrossOriginOpenerPolicy(value);

        return this;
    }

    /// <summary>
    /// The HTTP Cross-Origin-Embedder-Policy (COEP) response header configures embedding
    /// cross-origin resources into the document.
    /// Source: https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Cross-Origin-Embedder-Policy
    /// </summary>
    /// <param name="value">
    /// The HTTP Cross-Origin-Embedder-Policy response header value.
    /// </param>
    /// <remarks>
    /// Defaults to "require-corp" (<see cref="CrossOriginEmbedderPolicy.CrossOriginEmbedderOptions.RequireCorp"/>)
    /// which means that "Only requests from the same Origin (i.e. scheme + host + port) can read the resource."
    /// Note that "require-corp" is only meaningful alongside a Cross-Origin-Resource-Policy header, so
    /// <see cref="UseCrossOriginResourcePolicy"/> must also be called. This is enforced when the
    /// configuration is validated, which happens as the request pipeline is built.
    ///</remarks>
    public SecureHeadersBuilder UseCrossOriginEmbedderPolicy(
        CrossOriginEmbedderPolicy.CrossOriginEmbedderOptions value =
            CrossOriginEmbedderPolicy.CrossOriginEmbedderOptions.RequireCorp)
    {
        _configuration.UseCrossOriginEmbedderPolicy = true;
        _configuration.CrossOriginEmbedderPolicy = new CrossOriginEmbedderPolicy(value);

        return this;
    }

    /// <summary>
    /// The HTTP Reporting-Endpoints response header allows website administrators to specify one or more endpoints
    /// that can be sent reports generated by the Reporting API.
    /// The endpoints can be used, for example, as targets for sending CSP violation reports, Cross-Origin-Opener-Policy
    /// reports, or other generic violations.
    /// </summary>
    /// <param name="endpoints">
    /// A <see cref="Dictionary{TKey,TValue}"/> (where TKey is a string, and TValue is a Uri) which contains the endpoint
    /// names (string) and ABSOLUTE URLs for those endpoints
    /// </param>
    /// <remarks>
    /// The <see cref="ContentSecurityPolicyConfiguration"/> header makes use of this header's values via the
    /// <see cref="ContentSecurityPolicyConfiguration.ReportTo"/> property.
    /// If the value of the <see cref="ContentSecurityPolicyConfiguration.ReportTo"/> property isn't set to one of the
    /// endpoints listed in this header, then the browser will fail to send the CCP violation report.
    /// Note: As of January 7th, 2025: this header is marked as experimental and is not recommended by the OWASP Secure
    /// Headers project.
    /// </remarks>
    public SecureHeadersBuilder UseReportingEndpointsPolicy(Dictionary<string, Uri> endpoints)
    {
        _configuration.UseReportingEndPoints = true;
        _configuration.ReportingEndpointsPolicy = new ReportingEndpointsPolicy(endpoints);

        return this;
    }

    /// <summary>
    /// Used to set a list of URLs that we want the middleware to NOT operate on
    /// </summary>
    /// <param name="urlsToIgnore">
    /// A list of URLs that we the middleware to not operate on
    /// </param>
    /// <remarks>
    /// Supplying a null list is a no-op: the existing list is left alone rather than being
    /// replaced with null.
    /// </remarks>
    public SecureHeadersBuilder SetUrlsToIgnore(List<string> urlsToIgnore = null)
    {
        if (urlsToIgnore != null)
        {
            _configuration.UrlsToIgnore = urlsToIgnore;
        }

        return this;
    }

    /// <summary>
    /// Configures custom event IDs for logging to avoid conflicts with application event IDs
    /// </summary>
    /// <param name="loggingConfig">Custom logging configuration with specific event IDs</param>
    public SecureHeadersBuilder WithLoggingEventIds(SecureHeadersLoggingConfiguration loggingConfig)
    {
        ObjectGuardClauses.ObjectCannotBeNull(loggingConfig, nameof(loggingConfig),
            "cannot be null when configuring logging event IDs");

        _configuration.LoggingConfiguration = loggingConfig;

        return this;
    }

    /// <summary>
    /// Configures event IDs using a base offset to avoid conflicts with application event IDs.
    /// This creates event IDs starting from the base (e.g., baseEventId=5000 creates 5001, 5002, etc.)
    /// </summary>
    /// <param name="baseEventId">Base event ID to offset from (recommended: use multiples of 1000)</param>
    /// <example>
    /// <code>
    /// app.UseSecureHeadersMiddleware(opt =>
    /// {
    ///     opt.UseHsts();
    ///     opt.WithLoggingEventIdBase(5000); // Creates event IDs 5001, 5002, etc.
    /// });
    /// </code>
    /// </example>
    public SecureHeadersBuilder WithLoggingEventIdBase(int baseEventId)
    {
        _configuration.LoggingConfiguration = SecureHeadersLoggingConfiguration.CreateWithBaseEventId(baseEventId);

        return this;
    }

    /// <summary>
    /// Enables the Clear-Site-Data header for all responses with OWASP recommended defaults
    /// </summary>
    /// <param name="directiveOptions">The directive options to include (defaults to cache, cookies, storage)</param>
    /// <remarks>
    /// This enables Clear-Site-Data for all responses. For path-specific configuration,
    /// use <see cref="UseClearSiteDataForPaths"/> or <see cref="AddClearSiteDataPath"/> instead.
    /// </remarks>
    public SecureHeadersBuilder UseClearSiteData(params ClearSiteDataOptions[] directiveOptions)
    {
        // Use OWASP recommended defaults if no options provided
        if (directiveOptions.Length == 0)
        {
            directiveOptions = [ClearSiteDataOptions.cache, ClearSiteDataOptions.cookies, ClearSiteDataOptions.storage];
        }

        _configuration.UseClearSiteData = true;

        var clearSiteDataConfig = new ClearSiteDataConfiguration(directiveOptions);
        var pathConfig = new Dictionary<string, ClearSiteDataConfiguration>();

        _configuration.ClearSiteDataPathConfiguration = new ClearSiteDataPathConfiguration(
            pathConfig, clearSiteDataConfig);

        return this;
    }

    /// <summary>
    /// Configures path-specific Clear-Site-Data header behavior
    /// </summary>
    /// <param name="pathConfigurations">Dictionary mapping paths to their Clear-Site-Data directive options</param>
    /// <param name="defaultConfiguration">Default directives for non-matching paths (optional)</param>
    /// <remarks>
    /// Only the specified paths will receive Clear-Site-Data headers unless a default configuration is provided.
    /// </remarks>
    public SecureHeadersBuilder UseClearSiteDataForPaths(
        Dictionary<string, ClearSiteDataOptions[]> pathConfigurations,
        ClearSiteDataOptions[] defaultConfiguration = null)
    {
        ObjectGuardClauses.ObjectCannotBeNull(pathConfigurations, nameof(pathConfigurations),
            $"{nameof(pathConfigurations)} cannot be null");

        _configuration.UseClearSiteData = true;

        var configuredPaths = new Dictionary<string, ClearSiteDataConfiguration>();
        foreach (var kvp in pathConfigurations)
        {
            configuredPaths[kvp.Key] = new ClearSiteDataConfiguration(kvp.Value);
        }

        ClearSiteDataConfiguration defaultConfig = null;
        if (defaultConfiguration != null && defaultConfiguration.Length > 0)
        {
            defaultConfig = new ClearSiteDataConfiguration(defaultConfiguration);
        }

        _configuration.ClearSiteDataPathConfiguration = new ClearSiteDataPathConfiguration(
            configuredPaths, defaultConfig);

        return this;
    }

    /// <summary>
    /// Adds a path-specific Clear-Site-Data configuration
    /// </summary>
    /// <param name="path">The path to configure</param>
    /// <param name="directiveOptions">The directive options for this path</param>
    /// <remarks>
    /// This method can be called multiple times to configure different paths.
    /// Use this for fluent path-by-path configuration.
    /// </remarks>
    public SecureHeadersBuilder AddClearSiteDataPath(
        string path,
        params ClearSiteDataOptions[] directiveOptions)
    {
        HeaderValueGuardClauses.StringCannotBeNullOrWhitsSpace(path, nameof(path));
        ObjectGuardClauses.ObjectCannotBeNull(directiveOptions, nameof(directiveOptions),
            $"{nameof(directiveOptions)} cannot be null");

        if (directiveOptions.Length == 0)
        {
            ArgumentExceptionHelper.RaiseException(nameof(directiveOptions));
        }

        _configuration.UseClearSiteData = true;

        // Initialize if not already configured
        if (_configuration.ClearSiteDataPathConfiguration == null)
        {
            var initialPaths = new Dictionary<string, ClearSiteDataConfiguration>();
            _configuration.ClearSiteDataPathConfiguration = new ClearSiteDataPathConfiguration(initialPaths);
        }

        // Create new configuration with the additional path. PathConfigurations is a
        // FrozenDictionary, and repeated calls to this method must accumulate rather than replace.
        var existingPaths = new Dictionary<string, ClearSiteDataConfiguration>();
        foreach (var kvp in _configuration.ClearSiteDataPathConfiguration.PathConfigurations)
        {
            existingPaths[kvp.Key] = kvp.Value;
        }

        existingPaths[path] = new ClearSiteDataConfiguration(directiveOptions);

        _configuration.ClearSiteDataPathConfiguration = new ClearSiteDataPathConfiguration(
            existingPaths, _configuration.ClearSiteDataPathConfiguration.DefaultConfiguration);

        return this;
    }

    /// <summary>
    /// Used to set the Content Security Policy URIs for a given <see cref="CspUriType"/>
    /// </summary>
    /// <remarks>
    /// The URIs are applied to every Content Security Policy configured so far: the enforcing
    /// policy from <see cref="UseContentSecurityPolicy"/> or
    /// <see cref="UseDefaultContentSecurityPolicy"/>, and the report-only policy from
    /// <see cref="UseContentSecurityPolicyReportUriOnly"/>. This is a no-op if neither has been
    /// called yet.
    /// </remarks>
    public SecureHeadersBuilder SetCspUris(List<ContentSecurityPolicyElement> baseUri, CspUriType cspUriType)
    {
        _configuration.ContentSecurityPolicyConfiguration?.SetCspUri(baseUri, cspUriType);

        // The report-only policy gets its own copy, so that adding to one policy's list later
        // cannot change the other policy.
        _configuration.ContentSecurityPolicyReportOnlyConfiguration?.SetCspUri([.. baseUri], cspUriType);

        return this;
    }

    /// <summary>
    /// Used to set up the Content Security Policy Sandbox for a given or multiple
    /// <see cref="CspSandboxType"/>s
    /// </summary>
    /// <remarks>
    /// The sandbox is applied to every Content Security Policy configured so far, in the same way
    /// as <see cref="SetCspUris"/>. This is a no-op if no policy has been configured yet.
    /// </remarks>
    public SecureHeadersBuilder SetCspSandBox(params CspSandboxType[] sandboxType)
    {
        _configuration.ContentSecurityPolicyConfiguration?.SetSandbox(sandboxType);
        _configuration.ContentSecurityPolicyReportOnlyConfiguration?.SetSandbox([.. sandboxType]);

        return this;
    }
}
