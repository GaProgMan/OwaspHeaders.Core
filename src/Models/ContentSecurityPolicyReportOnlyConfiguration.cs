using OwaspHeaders.Core.Helpers;

namespace OwaspHeaders.Core.Models
{
    /// <summary>
    /// Represents a Report-Only CSP rule set. See the following link for more information:
    /// https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Content-Security-Policy-Report-Only
    /// </summary>
    public class ContentSecurityPolicyReportOnlyConfiguration : ContentSecurityPolicyConfiguration
    {
        public ContentSecurityPolicyReportOnlyConfiguration(string pluginTypes, bool blockAllMixedContent,
            bool upgradeInsecureRequests, string referrer, string reportUri)
            : base(pluginTypes, blockAllMixedContent, upgradeInsecureRequests, referrer, reportUri)
        {
        }

        public new string BuildHeaderValue()
        {
            if (string.IsNullOrWhiteSpace(ReportUri))
            {
                // We cannot have an empty ReportUri in a Report-Uri only CSP
                // Whilst this doesn't break the spec, as it states:
                // "The CSP report-uri directive should be used with this header,
                // otherwise this header will be an expensive no-op machine"
                // the decision has been taken to raise an exception here when
                // an empty one has been supplied.
                // This decision was taken to ensure that the Report-Uri will
                // actually be called - otherwise this response header is useless
                ArgumentExceptionHelper.RaiseException(nameof(ReportUri));
            }

            return base.BuildHeaderValue();
        }
    }
}
