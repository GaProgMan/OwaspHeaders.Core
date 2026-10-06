namespace OwaspHeaders.Core.Extensions;

/// <summary>
/// Content Security Policy helpers exposed as extension methods on
/// <see cref="SecureHeadersMiddlewareConfiguration"/>.
/// </summary>
/// <remarks>
/// These forward to the equivalent instance methods on <see cref="SecureHeadersBuilder"/>, which
/// own the real implementations. Prefer configuring the middleware in place:
/// <code>app.UseSecureHeadersMiddleware(opt => opt.UseContentSecurityPolicy().SetCspUris(...));</code>
/// </remarks>
public static class ContentSecurityPolicyExtensions
{
    /// <inheritdoc cref="SecureHeadersBuilder.SetCspUris"/>
    public static SecureHeadersMiddlewareConfiguration SetCspUris(
        this SecureHeadersMiddlewareConfiguration config,
        List<ContentSecurityPolicyElement> baseUri,
        CspUriType cspUriType)
        => new SecureHeadersBuilder(config).SetCspUris(baseUri, cspUriType).Build();

    /// <inheritdoc cref="SecureHeadersBuilder.SetCspSandBox"/>
    public static SecureHeadersMiddlewareConfiguration SetCspSandBox
        (this SecureHeadersMiddlewareConfiguration config, params CspSandboxType[] sandboxType)
        => new SecureHeadersBuilder(config).SetCspSandBox(sandboxType).Build();
}
