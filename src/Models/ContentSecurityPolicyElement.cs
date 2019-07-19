using OwaspHeaders.Core.Enums;

namespace OwaspHeaders.Core.Models
{
    public class ContentSecurityPolicyElement
    {
        public CspCommandType CommandType { get; set; }
        public string DirectiveOrUri { get; set; }
    }
}