namespace OwaspHeaders.Core.Enums;

/// <summary>
/// Represents the valid directive options for the Clear-Site-Data header.
/// </summary>
/// <remarks>Please note: these enum values are named after Clear-Site-Data directive options
/// exactly. This is so that we can use the value as a string, without having to
/// do any C# string magic (and waste cycles doing so) to get the right names.
/// This does mean that Rider (et al.) will tell you that the naming convention
/// here is non-standard.</remarks>
public enum ClearSiteDataOptions
{
    /// <summary>
    /// Clears browser cache for the origin
    /// </summary>
    cache,

    /// <summary>
    /// Clears all cookies for the origin
    /// </summary>
    cookies,

    /// <summary>
    /// Clears DOM storage (localStorage, sessionStorage, IndexedDB) for the origin
    /// </summary>
    storage,

    /// <summary>
    /// Represents "*" - clears all data types (takes precedence over other directives)
    /// </summary>
    wildcard
}
