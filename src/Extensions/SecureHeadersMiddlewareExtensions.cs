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
        return new SecureHeadersBuilder()
            .UseRecommendedDefaults()
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

        return AddMiddleware(builder, config ?? BuildDefaultConfiguration(urlIgnoreList));
    }

    /// <summary>
    /// Extension method to include the <see cref="SecureHeadersMiddleware" /> in an instance of
    /// an <see cref="IApplicationBuilder" />, using the headers recommended by OWASP.
    /// </summary>
    /// <param name="builder">
    /// The instance of the <see cref="IApplicationBuilder" /> to use
    /// </param>
    /// <returns>
    /// The <see cref="IApplicationBuilder"/> with the <see cref="SecureHeadersMiddleware" /> added
    /// </returns>
    /// <remarks>
    /// This is the one-line setup. It is equivalent to calling the overload which takes a
    /// configure delegate and calling <see cref="SecureHeadersBuilder.UseRecommendedDefaults"/>
    /// on the builder it hands you.
    /// </remarks>
    public static IApplicationBuilder UseSecureHeadersMiddleware(this IApplicationBuilder builder)
    {
        ObjectGuardClauses.ObjectCannotBeNull(builder, nameof(builder),
            "cannot be null when setting up OWASP Secure Headers in OwaspHeaders.Core");

        return AddMiddleware(builder, new SecureHeadersBuilder().UseRecommendedDefaults().Build());
    }

    /// <summary>
    /// Extension method to include the <see cref="SecureHeadersMiddleware" /> in an instance of
    /// an <see cref="IApplicationBuilder" />, configured by the supplied delegate.
    /// </summary>
    /// <param name="builder">
    /// The instance of the <see cref="IApplicationBuilder" /> to use
    /// </param>
    /// <param name="configure">
    /// An action which configures the <see cref="SecureHeadersBuilder"/> describing which headers
    /// to emit, and how
    /// </param>
    /// <returns>
    /// The <see cref="IApplicationBuilder"/> with the <see cref="SecureHeadersMiddleware" /> added
    /// </returns>
    /// <remarks>
    /// <para>
    /// The builder handed to <paramref name="configure"/> starts empty: it does not pre-apply the
    /// OWASP recommended headers. Call
    /// <see cref="SecureHeadersBuilder.UseRecommendedDefaults"/> first to start from that set.
    /// </para>
    /// <para>
    /// <paramref name="configure"/> is invoked once, here, as the request pipeline is being built.
    /// Holding on to the builder and mutating it later is not supported.
    /// </para>
    /// <example>
    /// <code>
    /// app.UseSecureHeadersMiddleware(opt =>
    /// {
    ///     opt.UseRecommendedDefaults();
    ///     opt.SetUrlsToIgnore(["/health"]);
    /// });
    /// </code>
    /// </example>
    /// </remarks>
    public static IApplicationBuilder UseSecureHeadersMiddleware(this IApplicationBuilder builder,
        Action<SecureHeadersBuilder> configure)
    {
        ObjectGuardClauses.ObjectCannotBeNull(builder, nameof(builder),
            "cannot be null when setting up OWASP Secure Headers in OwaspHeaders.Core");
        ObjectGuardClauses.ObjectCannotBeNull(configure, nameof(configure),
            "cannot be null when setting up OWASP Secure Headers in OwaspHeaders.Core");

        var secureHeadersBuilder = new SecureHeadersBuilder();
        configure(secureHeadersBuilder);

        return AddMiddleware(builder, secureHeadersBuilder.Build());
    }

    /// <summary>
    /// Validates the configuration and adds the middleware to the pipeline.
    /// </summary>
    /// <remarks>
    /// Every overload routes through here, so an invalid configuration throws from the line the
    /// consumer wrote in their own startup code, before the middleware is registered at all.
    /// The middleware validates again when it is constructed, which covers callers who reach
    /// for <c>UseMiddleware&lt;SecureHeadersMiddleware&gt;</c> directly.
    /// Nothing is logged here: this runs before the middleware owns a logger, and
    /// <see cref="IApplicationBuilder.ApplicationServices"/> is not guaranteed to be populated.
    /// </remarks>
    private static IApplicationBuilder AddMiddleware(
        IApplicationBuilder builder, SecureHeadersMiddlewareConfiguration config)
    {
        config.ValidateOrThrow();

        return builder.UseMiddleware<SecureHeadersMiddleware>(config);
    }
}
