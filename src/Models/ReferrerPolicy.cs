namespace OwaspHeaders.Core.Models
{
    public class ReferrerPolicy : IConfigurationBase
    {
        public ReferrerPolicyOptions ReferrerPolicyOption { get; init; }

        /// <summary>
        /// Protected constructor, we can no longer create instances of this class without
        /// using the public constructor
        /// </summary>
        [ExcludeFromCodeCoverage]
        protected ReferrerPolicy() { }

        public ReferrerPolicy(ReferrerPolicyOptions referrerPolicyOption)
        {
            ReferrerPolicyOption = referrerPolicyOption;
        }

        public string BuildHeaderValue()
        {
            switch (ReferrerPolicyOption)
            {
                case ReferrerPolicyOptions.noReferrer:
                    return "no-referrer";
                case ReferrerPolicyOptions.noReferrerWhenDowngrade:
                    return "no-referrer-when-downgrade";
                case ReferrerPolicyOptions.origin:
                    return "origin";
                case ReferrerPolicyOptions.originWhenCrossOrigin:
                    return "origin-when-cross-origin";
                case ReferrerPolicyOptions.sameOrigin:
                    return "same-origin";
                case ReferrerPolicyOptions.strictOrigin:
                    return "strict-origin";
                case ReferrerPolicyOptions.strictWhenCrossOrigin:
                    return "strict-origin-when-cross-origin";
                case ReferrerPolicyOptions.unsafeUrl:
                    return "unsafe-url";
                default:
                    ArgumentExceptionHelper.RaiseException(nameof(ReferrerPolicyOption));
                    break;
            }
            // We should never hit this return statement. It is included here
            // as the method NEEDs to return something.
            return ";";
        }
    }
}
