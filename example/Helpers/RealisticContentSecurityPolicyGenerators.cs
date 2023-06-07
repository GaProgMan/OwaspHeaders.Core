using OwaspHeaders.Core.Enums;
using OwaspHeaders.Core.Extensions;
using OwaspHeaders.Core.Models;

namespace example.Helpers;

/// <summary>
/// This class is useful for testing the Secure Headers middleware with a set of realistic
/// settings. It is currently (as of June 7th, 2023) not being used but I would prefer that
/// it sticks around as it would be very useful to use in the future.
/// This flies against standard Software Engineering practises. But this particular csproj
/// is only provided for developers to test the OwaspHeaders.Core project whilst they are
/// developing it (i.e. without having to push to GitHub, wait for a build, wait for a PR,
/// wait for a release, wait for NuGet to index, and wait for it to download).
/// </summary>
public static class RealisticContentSecurityPolicyGenerators
{
    /// <summary>
    /// Represents an instance of the <see cref="SecureHeadersMiddlewareConfiguration"/> with
    /// the Content-Security Policy from the OWASP homepage.
    /// </summary>
    /// <remarks>
    /// The instance of hte <see cref="SecureHeadersMiddlewareConfiguration"/> that this method
    /// returns, DOES NOT contain any other header values. The return value from this method is
    /// provided as a way of testing the CSP generation code. And should NOT be used in a live
    /// environment (unless you are replacing the OWASP website with ASP .NET Core 😛)
    /// </remarks>
    /// <returns>
    /// An instance of the <see cref="SecureHeadersMiddlewareConfiguration"/> with headers which
    /// represent the Content-Security Policy taken from the OWASP website homepage on May 15th, 2023
    /// </returns>
    public static SecureHeadersMiddlewareConfiguration GenerateOwaspHomePageCsp() =>
        SecureHeadersMiddlewareExtensions.BuildDefaultConfiguration()
            .UseContentSecurityPolicy()
            .SetCspUris(
                new List<ContentSecurityPolicyElement>
                {
                    new() { CommandType = CspCommandType.Directive, DirectiveOrUri = "self" },
                    new() { CommandType = CspCommandType.Uri, DirectiveOrUri = "https://api.github.com" },
                    new() { CommandType = CspCommandType.Uri, DirectiveOrUri = "https://*.githubusercontent.com" },
                    new() { CommandType = CspCommandType.Uri, DirectiveOrUri = "https://*.google-analytics.com" },
                    new()
                    {
                        CommandType = CspCommandType.Uri,
                        DirectiveOrUri = "https://owaspadmin.azurewebsites.net"
                    },
                    new() { CommandType = CspCommandType.Uri, DirectiveOrUri = "https://*.twimg.com" },
                    new() { CommandType = CspCommandType.Uri, DirectiveOrUri = "https://platform.twitter.com" },
                    new() { CommandType = CspCommandType.Uri, DirectiveOrUri = "https://www.youtube.com" },
                    new() { CommandType = CspCommandType.Uri, DirectiveOrUri = "https://*.doubleclick.net" }
                }, CspUriType.DefaultUri)
            .SetCspUris(
                new List<ContentSecurityPolicyElement>
                {
                    new() { CommandType = CspCommandType.Directive, DirectiveOrUri = "self" },
                    new() { CommandType = CspCommandType.Uri, DirectiveOrUri = "https://api.github.com" },
                    new() { CommandType = CspCommandType.Uri, DirectiveOrUri = "https://*.githubusercontent.com" },
                    new() { CommandType = CspCommandType.Uri, DirectiveOrUri = "https://*.google-analytics.com" },
                    new() { CommandType = CspCommandType.Uri, DirectiveOrUri = "https://owaspadmin.azurewebsites.net" },
                    new() { CommandType = CspCommandType.Uri, DirectiveOrUri = "https://*.twimg.com" },
                    new() { CommandType = CspCommandType.Uri, DirectiveOrUri = "https://platform.twitter.com" },
                    new() { CommandType = CspCommandType.Uri, DirectiveOrUri = "https://www.youtube.com" },
                    new() { CommandType = CspCommandType.Uri, DirectiveOrUri = "https://*.doubleclick.net" },
                }, CspUriType.FrameAncestors)
            .SetCspUris(
                new List<ContentSecurityPolicyElement>
                {
                    new() { CommandType = CspCommandType.Uri, DirectiveOrUri = "https://*.vuejs.org" },
                    new() { CommandType = CspCommandType.Uri, DirectiveOrUri = "https://*.stripe.com" },
                    new() { CommandType = CspCommandType.Uri, DirectiveOrUri = "https://*.wufoo.com" },
                    new() { CommandType = CspCommandType.Uri, DirectiveOrUri = "https://*.sched.com" },
                    new() { CommandType = CspCommandType.Uri, DirectiveOrUri = "https://*.google.com" },
                    new() { CommandType = CspCommandType.Uri, DirectiveOrUri = "https://*.twitter.com" },
                    new() { CommandType = CspCommandType.Uri, DirectiveOrUri = "https://www.youtube.com" },
                    new() { CommandType = CspCommandType.Uri, DirectiveOrUri = "https://w.soundcloud.com" },
                }, CspUriType.Frame)
            .SetCspUris(
                new List<ContentSecurityPolicyElement>
                {
                    new() { CommandType = CspCommandType.Directive, DirectiveOrUri = "self" },
                    new() { CommandType = CspCommandType.Directive, DirectiveOrUri = "unsafe-inline" },
                    new() { CommandType = CspCommandType.Directive, DirectiveOrUri = "unsafe-eval" },
                    new() { CommandType = CspCommandType.Uri, DirectiveOrUri = "https://fonts.googleapis.com" },
                    new() { CommandType = CspCommandType.Uri, DirectiveOrUri = "https://app.diagrams.net" },
                    new() { CommandType = CspCommandType.Uri, DirectiveOrUri = "https://cdnjs.cloudflare.com" },
                    new() { CommandType = CspCommandType.Uri, DirectiveOrUri = "https://cse.google.com" },
                    new() { CommandType = CspCommandType.Uri, DirectiveOrUri = "https://*.vuejs.org" },
                    new() { CommandType = CspCommandType.Uri, DirectiveOrUri = "https://*.stripe.com" },
                    new() { CommandType = CspCommandType.Uri, DirectiveOrUri = "https://*.wufoo.com" },
                    new() { CommandType = CspCommandType.Uri, DirectiveOrUri = "https://*.youtube.com" },
                    new() { CommandType = CspCommandType.Uri, DirectiveOrUri = "https://*.meetup.com" },
                    new() { CommandType = CspCommandType.Uri, DirectiveOrUri = "https://*.sched.com" },
                    new() { CommandType = CspCommandType.Uri, DirectiveOrUri = "https://*.google-analytics.com" },
                    new() { CommandType = CspCommandType.Uri, DirectiveOrUri = "https://unpkg.com" },
                    new() { CommandType = CspCommandType.Uri, DirectiveOrUri = "https://buttons.github.io" },
                    new() { CommandType = CspCommandType.Uri, DirectiveOrUri = "https://www.google.com" },
                    new() { CommandType = CspCommandType.Uri, DirectiveOrUri = "https://*.gstatic.com" },
                    new() { CommandType = CspCommandType.Uri, DirectiveOrUri = "https://*.twitter.com" },
                    new() { CommandType = CspCommandType.Uri, DirectiveOrUri = "https://*.twimg.com" },
                }, CspUriType.Script)
            .SetCspUris(
                new List<ContentSecurityPolicyElement>
                {
                    new() { CommandType = CspCommandType.Directive, DirectiveOrUri = "self" },
                    new() { CommandType = CspCommandType.Directive, DirectiveOrUri = "unsafe-inline" },
                    new() { CommandType = CspCommandType.Uri, DirectiveOrUri = "https://*.gstatic.com" },
                    new() { CommandType = CspCommandType.Uri, DirectiveOrUri = "https://cdnjs.cloudflare.com" },
                    new() { CommandType = CspCommandType.Uri, DirectiveOrUri = "https://www.google.com" },
                    new() { CommandType = CspCommandType.Uri, DirectiveOrUri = "https://fonts.googleapis.com" },
                    new() { CommandType = CspCommandType.Uri, DirectiveOrUri = "https://platform.twitter.com" },
                    new() { CommandType = CspCommandType.Uri, DirectiveOrUri = "https://*.twimg.com" },
                    new() { CommandType = CspCommandType.Uri, DirectiveOrUri = "data:" },
                }, CspUriType.Style)
            .SetCspUris(
                new List<ContentSecurityPolicyElement>
                {
                    new() { CommandType = CspCommandType.Directive, DirectiveOrUri = "self" },
                    new() { CommandType = CspCommandType.Uri, DirectiveOrUri = "fonts.gstatic.com" }
                }, CspUriType.Font)
            .SetCspUris(
                new List<ContentSecurityPolicyElement>
                {
                    new() { CommandType = CspCommandType.Directive, DirectiveOrUri = "self" },
                    new() { CommandType = CspCommandType.Uri, DirectiveOrUri = "https://pay.google.com" }
                }, CspUriType.Manifest)
            .SetCspUris(
                new List<ContentSecurityPolicyElement>
                {
                    new() { CommandType = CspCommandType.Directive, DirectiveOrUri = "self" },
                    new() { CommandType = CspCommandType.Uri, DirectiveOrUri = "https://*.globalappsec.org" },
                    new() { CommandType = CspCommandType.Uri, DirectiveOrUri = "data:" },
                    new() { CommandType = CspCommandType.Uri, DirectiveOrUri = "www.w3.org" },
                    new() { CommandType = CspCommandType.Uri, DirectiveOrUri = "https://licensebuttons.net" },
                    new() { CommandType = CspCommandType.Uri, DirectiveOrUri = "https://img.shields.io" },
                    new() { CommandType = CspCommandType.Uri, DirectiveOrUri = "https://*.twitter.com" },
                    new() { CommandType = CspCommandType.Uri, DirectiveOrUri = "https://github.githubassets.com" },
                    new() { CommandType = CspCommandType.Uri, DirectiveOrUri = "https://*.twimg.com" },
                    new() { CommandType = CspCommandType.Uri, DirectiveOrUri = "https://platform.twitter.com" },
                    new() { CommandType = CspCommandType.Uri, DirectiveOrUri = "https://*.githubusercontent.com" },
                    new() { CommandType = CspCommandType.Uri, DirectiveOrUri = "https://*.vercel.app" },
                    new() { CommandType = CspCommandType.Uri, DirectiveOrUri = "https://*.cloudfront.net" },
                    new() { CommandType = CspCommandType.Uri, DirectiveOrUri = "https://*.coreinfrastructure.org" },
                    new()
                    {
                        CommandType = CspCommandType.Uri,
                        DirectiveOrUri = "https://*.securityknowledgeframework.org"
                    },
                    new() { CommandType = CspCommandType.Uri, DirectiveOrUri = "https://badges.gitter.im" },
                    new() { CommandType = CspCommandType.Uri, DirectiveOrUri = "https://travis-ci.org" },
                    new() { CommandType = CspCommandType.Uri, DirectiveOrUri = "https://api.travis-ci.org" },
                    new() { CommandType = CspCommandType.Uri, DirectiveOrUri = "https://s3.amazonaws.com" },
                    new() { CommandType = CspCommandType.Uri, DirectiveOrUri = "https://snyk.io" },
                    new() { CommandType = CspCommandType.Uri, DirectiveOrUri = "https://coveralls.io" },
                    new() { CommandType = CspCommandType.Uri, DirectiveOrUri = "https://requires.io" },
                    new() { CommandType = CspCommandType.Uri, DirectiveOrUri = "https://github.com" },
                    new() { CommandType = CspCommandType.Uri, DirectiveOrUri = "https://*.googleapis.com" },
                    new() { CommandType = CspCommandType.Uri, DirectiveOrUri = "https://*.google.com" },
                    new() { CommandType = CspCommandType.Uri, DirectiveOrUri = "https://*.gstatic.com" },
                }, CspUriType.Img);
}
