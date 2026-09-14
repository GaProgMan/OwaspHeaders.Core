namespace OwaspHeaders.Core.Models;

/// <summary>
/// Represents a Content Security Policy element that can be either a CSP directive keyword or a URI source.
/// </summary>
public class ContentSecurityPolicyElement
{
    /// <summary>
    /// Specifies whether this element represents a CSP directive keyword or a URI source.
    /// Use <see cref="CspCommandType.Directive"/> for CSP keywords like 'self', 'unsafe-inline', etc.
    /// Use <see cref="CspCommandType.Uri"/> for URLs, domains, and URI schemes like 'https://example.com', 'data:', etc.
    /// </summary>
    public CspCommandType CommandType { get; init; }

    /// <summary>
    /// The actual directive keyword or URI value.
    /// For directives: "self", "unsafe-inline", "unsafe-eval", "none", etc. (without quotes)
    /// For URIs: "https://example.com", "data:", "blob:", "*.example.com", etc.
    /// </summary>
    /// <remarks>
    /// Required, because an element without a value renders as <c>''</c> for a directive or as an
    /// empty token for a URI, either of which quietly produces a malformed header. The guard in
    /// the initialiser covers what <c>required</c> cannot: an explicit null, a caller compiling
    /// without nullable reference types, and the empty or whitespace strings.
    /// </remarks>
    /// <exception cref="ArgumentException">
    /// Thrown when the value is null, empty or whitespace.
    /// </exception>
    public required string DirectiveOrUri
    {
        get;
        init
        {
            HeaderValueGuardClauses.StringCannotBeNullOrWhiteSpace(value, nameof(DirectiveOrUri));
            field = value;
        }
    }
}
