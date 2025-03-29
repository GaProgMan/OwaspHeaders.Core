using Microsoft.AspNetCore.Builder;

namespace OwaspHeaders.Core.Extensions;

/// <summary>
/// Extension methods to allow us to easily build the middleware
/// </summary>
public static class SecureHeadersMiddlewareExtensions
{
    /// <summary>
    /// Builds an instance of the <see cref="SecureHeadersMiddlewareConfiguration"/>
    /// with default values. The default values are supplied via the calls to `Use...`
    /// on each of the headers, please see the comments on those methods for the
    /// current default values that they supply.
    /// </summary>
    /// <remarks>
    /// This method sets up all the headers which are recommended by OWASP, using
    /// default values that they recommend in their best practises. Please see the following
    /// url for the current best practises:
    /// https://www.owasp.org/index.php/OWASP_Secure_Headers_Project#tab=Best_Practices
    /// </remarks>
    public static SecureHeadersMiddlewareConfiguration BuildDefaultConfiguration(
        List<string> urlIgnoreList = null)
    {
        return SecureHeadersMiddlewareBuilder
            .CreateBuilder()
            .UseHsts()
            .UseXFrameOptions()
            .UseContentTypeOptions()
            .UseDefaultContentSecurityPolicy()
            .UsePermittedCrossDomainPolicies()
            .UseReferrerPolicy()
            .UseCacheControl()
            .UseXssProtection()
            .UseCrossOriginResourcePolicy()
            .UseCrossOriginOpenerPolicy()
            .UseCrossOriginEmbedderPolicy()
            // When the OWASP Secure Headers project recommends the use of the Reporting-Endpoints header, we will
            // enable it here
            .SetUrlsToIgnore(urlIgnoreList)
            .Build();
    }

    /// <summary>
    /// Extension method to include the <see cref="SecureHeadersMiddleware" /> in
    /// an instance of an <see cref="IApplicationBuilder" />.
    /// This works in the same way was the MVC, Static files, etc. middleware
    /// </summary>
    /// <param name="builder">
    /// The instance of the <see cref="IApplicationBuilder" /> to use
    /// </param>
    /// <param name="config">
    /// [OPTIONAL] An instance of the <see cref="SecureHeadersMiddlewareConfiguration" />
    /// containing all the config for each request
    /// </param>
    /// <param name="urlIgnoreList">
    /// A list of URLs to ignore when processes requests. For example, to disable the entire
    /// middleware when the user accesses "/path-to-ignore", add this to the list and the
    /// middleware will be disabled for that URL.
    /// </param>
    /// <returns>
    /// The <see cref="IApplicationBuilder"/> with the <see cref="SecureHeadersMiddleware" /> added
    /// </returns>
    /// <remarks>
    /// If an instance of <see cref="SecureHeadersMiddlewareConfiguration"/> is not provided,
    /// then the default value from <see cref="BuildDefaultConfiguration"/> will be provided.
    /// </remarks>
    public static IApplicationBuilder UseSecureHeadersMiddleware(this IApplicationBuilder builder,
        SecureHeadersMiddlewareConfiguration config = null, List<string> urlIgnoreList = null)
    {
        ObjectGuardClauses.ObjectCannotBeNull(builder, nameof(builder),
            "cannot be null when setting up OWASP Secure Headers in OwaspHeaders.Core");
        return builder.UseMiddleware<SecureHeadersMiddleware>(config ?? BuildDefaultConfiguration(urlIgnoreList));
    }
}
