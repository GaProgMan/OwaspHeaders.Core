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
