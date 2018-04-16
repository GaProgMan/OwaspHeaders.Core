using Microsoft.AspNetCore.Builder;
using OwaspHeaders.Core.Models;

namespace OwaspHeaders.Core.Extensions
{
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
        /// This method sets up all of the headers which are recommended by OWASP, using
        /// default values that they recommend in their best practises. Please see the following
        /// url for the current best practises:
        /// https://www.owasp.org/index.php/OWASP_Secure_Headers_Project#tab=Best_Practices
        /// </remarks>
        public static SecureHeadersMiddlewareConfiguration BuildDefaultConfiguration()
        {
            return SecureHeadersMiddlewareBuilder
                .CreateBuilder()
                .UseHsts()
                .UseXFrameOptions()
                .UseXSSProtection()
                .UseContentTypeOptions()
                .UseContentDefaultSecurityPolicy()
                .UsePermittedCrossDomainPolicies()
                .UseReferrerPolicy()
                .UseExpectCt("https://gaprogman.com/report", enforce:true)
                .RemovePoweredByHeader()
                .Build();
        }
        
        /// <summary>
        /// Extention method to include the <see cref="SecureHeadersMiddleware" /> in
        /// an instance of an <see cref="IApplicationBuilder" />.
        /// This works in the same way was the MVC, Static files, etc. middleware
        /// </summary>
        /// <param name="builder">The instance of the <see cref="IApplicationBuilder" /> to use</param>
        /// <param name="config">An instance of the <see cref="SecureHeadersMiddlewareConfiguration" /> containing all of the config for each request </param>
        /// <returns>The <see cref="IApplicationBuilder"/> with the <see cref="SecureHeadersMiddleware" /> added</returns>
        public static IApplicationBuilder UseSecureHeadersMiddleware(this IApplicationBuilder builder, SecureHeadersMiddlewareConfiguration config)
        {
            return builder.UseMiddleware<SecureHeadersMiddleware>(config);
        }
    }
}
