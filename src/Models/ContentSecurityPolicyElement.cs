namespace OwaspHeaders.Core.Models;

public class ContentSecurityPolicyElement
{
    public CspCommandType CommandType { get; init; }
    public string DirectiveOrUri { get; init; }
}
