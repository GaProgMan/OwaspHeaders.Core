using OwaspHeaders.Core.Enums;
using OwaspHeaders.Core.Models;

namespace OwaspHeaders.Core.Helpers
{
    public static class ContentSecurityPolicyHelpers
    {
        /// <summary>
        /// Used to create the default "self" CSP directive.
        /// This can be applied to any of the CSP rules individually
        /// or to the 'default-src' rule to cover them all.
        /// </summary>
        /// <returns>
        /// A new instance of the <see cref="ContentSecurityPolicyElement"/>
        /// class which represents a 'self' CSP rule.
        /// </returns>
        public static ContentSecurityPolicyElement CreateSelfDirective()
        {
            return new ContentSecurityPolicyElement
            {
                CommandType = CspCommandType.Directive,
                DirectiveOrUri = "self"
            };
        }
    }
}