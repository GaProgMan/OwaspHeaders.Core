namespace OwaspHeaders.Core.Models;

/// <summary>
/// This class represents some of the most commonly used Cache Control
/// directives. For more information on this header, please see:
/// https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Cache-Control
/// </summary>
public class CacheControl : IConfigurationBase
{
    /// <summary>
    /// Whether all or part of the HTTP response message is intended for a
    /// single user and must not be cached by a shared cache.
    /// </summary>
    /// <remarks>
    /// The following is taken from the MDN article for cache-control
    ///    If you forget to add private to a response with personalized content,
    ///    then that response can be stored in a shared cache and end up being
    ///    reused for multiple users, which can cause personal information to
    ///    leak.
    /// Source: https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Cache-Control#private
    /// </remarks>
    public bool Private { get; }

    /// <summary>
    /// The maximum age, specified in seconds, that the HTTP client is willing
    /// to accept a response.
    /// </summary>
    /// <remarks>
    /// A negative value is not a valid max-age, so it is sent as max-age=0
    /// </remarks>
    public int MaxAge { get; }

    /// <summary>
    /// Represents whether the response can be cached. If the response cannot be
    /// cached, then the origin server
    /// must be contacted for every request.
    /// </summary>
    /// <remarks>
    /// The following is taken from the MDN article for cache-control
    ///   Note that no-cache does not mean "don't cache". no-cache allows caches
    ///   to store a response but requires them to revalidate it before reuse.
    ///   If the sense of "don't cache" that you want is actually "don't store",
    ///   then no-store is the directive to use.
    /// Source: https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Cache-Control#no-cache
    /// </remarks>
    public bool NoCache { get; }

    /// <summary>
    /// Represents whether the response can be stored in caches and used whilst still
    /// fresh
    /// </summary>
    /// <remarks>
    /// This is used alongside the MaxAge directive
    /// </remarks>
    public bool MustRevalidate { get; }

    /// <summary>
    /// Represents whether the response can be stored anywhere (i.e. either public
    /// or private caches)
    /// </summary>
    public bool NoStore { get; }

    public CacheControl(bool @private, int maxAge = 0, bool noCache = true,
        bool noStore = true, bool mustRevalidate = false)
    {
        Private = @private;
        MaxAge = maxAge;
        NoCache = noCache;
        NoStore = noStore;
        MustRevalidate = mustRevalidate;
    }

    /// <summary>
    /// Builds the HTTP header value
    /// </summary>
    /// <remarks>
    /// Every directive whose flag is set is included, so the flags can be combined. The
    /// default values produce "no-cache, no-store, max-age=0", which ASP.NET Core's
    /// antiforgery system accepts without overriding the header or logging a warning.
    /// See https://github.com/GaProgMan/OwaspHeaders.Core/issues/261
    /// </remarks>
    /// <returns>A string representing the HTTP header value</returns>
    public string BuildHeaderValue()
    {
        var directives = new List<string>();
        if (Private)
        {
            directives.Add("private");
        }

        if (NoCache)
        {
            directives.Add("no-cache");
        }

        if (NoStore)
        {
            directives.Add("no-store");
        }

        directives.Add($"max-age={Math.Max(MaxAge, 0)}");

        if (MustRevalidate)
        {
            directives.Add("must-revalidate");
        }

        return string.Join(", ", directives);
    }
}
