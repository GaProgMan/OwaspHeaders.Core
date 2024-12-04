namespace OwaspHeaders.Core.Enums;

/// <remarks>
/// Please note: these enum values are named after the X-Permitted-Cross-Domain-Options
/// values exactly. This is so that we can use the value as a string, without having to
/// do any C# string magic (and waste cycles doing so) to get the right names. 
/// This does mean that Rider (et al.) will tell you that the naming convention
/// here is non-standard.</remarks>
public enum XPermittedCrossDomainOptionValue
{
    none,
    masterOnly,
    byContentType,
    byFtpFileType,
    all
};