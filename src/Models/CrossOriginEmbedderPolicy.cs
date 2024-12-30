namespace OwaspHeaders.Core.Models;

/// <summary>
/// Cross-Origin-Embedder-Policy
/// This response header (also named COEP) prevents a document from loading any
/// cross-origin resources that don’t explicitly grant the document permission
/// (source Mozilla MDN).
/// 💡 To fully understand where CORP and COEP work:
/// - CORP applies on the loaded resource side (resource owner).
/// - COEP applies on the “loader” of the resource side (consumer of the resource).
/// MDN Source: https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Cross-Origin-Embedder-Policy
/// </summary>
public class CrossOriginEmbedderPolicy : IConfigurationBase
{
    /// <summary>
    /// Cross-Origin-Embedder-Policy
    /// This response header (also named COEP) prevents a document from loading any
    /// cross-origin resources that don’t explicitly grant the document permission
    /// (source Mozilla MDN).
    /// 💡 To fully understand where CORP and COEP work:
    /// - CORP applies on the loaded resource side (resource owner).
    /// - COEP applies on the “loader” of the resource side (consumer of the resource).
    /// MDN Source: https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Cross-Origin-Embedder-Policy
    /// </summary>
    public CrossOriginEmbedderPolicy(CrossOriginEmbedderOptions value =
        CrossOriginEmbedderOptions.RequireCorp)
    {
        OptionValue = value;
    }

    /// <summary>
    /// Allows the document to fetch cross-origin resources without giving explicit permission
    /// through the CORS protocol or the Cross-Origin-Resource-Policy header (it is the default value).
    /// </summary>
    public const string UnsafeNoneValue = "unsafe-none";

    /// <summary>
    /// A document can only load resources from the same origin, or resources explicitly
    /// marked as loadable from another origin.
    /// </summary>
    public const string RequireCorp = "same-require-corp";

    public enum CrossOriginEmbedderOptions
    {
        /// <summary>
        /// <see cref="UnsafeNoneValue"/>
        /// </summary>
        UnsafeNone,

        /// <summary>
        /// <see cref="RequireCorp"/>
        /// </summary>
        RequireCorp
    };

    private CrossOriginEmbedderOptions OptionValue { get; }

    /// <summary>
    /// Builds the HTTP header value
    /// </summary>
    /// <returns>A string representing the HTTP header value</returns>
    public string BuildHeaderValue()
    {
        switch (OptionValue)
        {
            case CrossOriginEmbedderOptions.UnsafeNone:
                return UnsafeNoneValue;
            case CrossOriginEmbedderOptions.RequireCorp:
            default:
                return RequireCorp;
        }
    }
}
