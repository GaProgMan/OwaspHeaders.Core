namespace OwaspHeaders.Core.Models;

/// <summary>
/// Cross-Origin-Opener-Policy.
/// The following text was taken from the OWASP Secure Headers Project:
/// This response header (also named COOP) allows you to ensure a top-level
/// document does not share a browsing context group with cross-origin documents.
/// COOP will process-isolate your document and potential attackers can’t access
/// to your global object if they were opening it in a popup, preventing a
/// set of cross-origin attacks dubbed XS-Leaks (https://xsleaks.dev/)
/// (source Mozilla MDN (https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Cross-Origin-Opener-Policy)).


/// </summary>
public class CrossOriginOpenerPolicy : IConfigurationBase
{
    /// <summary>
    /// Cross-Origin-Opener-Policy.
    /// The following text was taken from the OWASP Secure Headers Project:
    /// This response header (also named COOP) allows you to ensure a top-level
    /// document does not share a browsing context group with cross-origin documents.
    /// COOP will process-isolate your document and potential attackers can’t access
    /// to your global object if they were opening it in a popup, preventing a
    /// set of cross-origin attacks dubbed XS-Leaks (https://xsleaks.dev/)
    /// (source Mozilla MDN (https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Cross-Origin-Opener-Policy)).
    /// </summary>
    public CrossOriginOpenerPolicy(CrossOriginOpenerOptions value =
        CrossOriginOpenerOptions.SameOrigin)
    {
        OptionValue = value;
    }

    /// <summary>
    /// Allows the document to be added to its opener’s browsing context group
    /// unless the opener itself has a COOP of same-origin or same-origin-allow-popups (it is the default value).
    /// </summary>
    private const string UnsafeNoneValue = "unsafe-none";

    /// <summary>
    /// Only requests from the same Site can read the resource.
    /// </summary>
    private const string SameOriginAllowPopupsValue = "same-origin-allow-popups";

    /// <summary>
    /// Retains references to newly opened windows or tabs which either
    /// don’t set COOP or which opt out of isolation by setting a COOP
    /// of unsafe-none
    /// </summary>
    private const string SameOriginValue = "same-origin";

    public enum CrossOriginOpenerOptions
    {
        /// <summary>
        /// <see cref="UnsafeNoneValue"/>
        /// </summary>
        UnsafeNone,

        /// <summary>
        /// <see cref="SameOriginAllowPopupsValue"/>
        /// </summary>
        SameOriginAllowPopups,

        /// <summary>
        /// <see cref="SameOriginValue"/>
        /// </summary>
        SameOrigin
    };

    public CrossOriginOpenerOptions OptionValue { get; }

    /// <summary>
    /// Builds the HTTP header value
    /// </summary>
    /// <returns>A string representing the HTTP header value</returns>
    public string BuildHeaderValue()
    {
        switch (OptionValue)
        {
            case CrossOriginOpenerOptions.SameOriginAllowPopups:
                return SameOriginAllowPopupsValue;
            case CrossOriginOpenerOptions.UnsafeNone:
                return UnsafeNoneValue;
            case CrossOriginOpenerOptions.SameOrigin:
            default:
                return SameOriginValue;
        }
    }
}
