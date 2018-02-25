using OwaspHeaders.Core.Enums;

namespace OwaspHeaders.Core.Models
{
    public class ContenSecurityPolicyElement
    {
        public CspCommandType CommandType { get; set; }
        public string DirectiveOrUri { get; set; }
    }
}