namespace OwaspHeaders.Core.Enums;

public enum CspCommandType
{
    /// <summary>
    /// Use for CSP keywords and directives such as 'self', 'unsafe-inline', 'unsafe-eval', 'strict-dynamic', 'none', etc.
    /// These values are typically enclosed in single quotes in the CSP header.
    /// </summary>
    Directive,
    
    /// <summary>
    /// Use for URLs, domains, and URI schemes such as 'https://example.com', 'data:', 'blob:', '*.example.com', etc.
    /// These values represent actual URIs or URI patterns that are allowed as sources.
    /// </summary>
    Uri
}
