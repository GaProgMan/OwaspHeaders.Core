namespace OwaspHeaders.Core.Models;

/// <summary>
/// Represents a Report-Only CSP rule set. See the following link for more information:
/// https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Content-Security-Policy-Report-Only
/// </summary>
public class ContentSecurityPolicyReportOnlyConfiguration : ContentSecurityPolicyConfiguration
{
    public ContentSecurityPolicyReportOnlyConfiguration(string pluginTypes, bool blockAllMixedContent,
        bool upgradeInsecureRequests, string referrer, string reportUri, string reportTo)
        : base(pluginTypes, blockAllMixedContent, upgradeInsecureRequests, referrer, reportUri, reportTo)
    {
    }

    // This is a _VERY_ temporary fix for marking the ReportUri property as deprecated
    // Because we're deprecating ReportUri but are using it throughout this class _AND_ we have warnings as errors, we need
    // to disable the CS0618 warning for the duration of this fix.
#pragma warning disable CS0618
    public new string BuildHeaderValue()
    {
        // We cannot have an empty ReportUri in a Report-Uri only CSP
        // Whilst this doesn't break the spec, as it states:
        // "The CSP report-uri directive should be used with this header,
        // otherwise this header will be an expensive no-op machine"
        // the decision has been taken to raise an exception here when
        // an empty one has been supplied.
        // This decision was taken to ensure that the Report-Uri will
        // actually be called - otherwise this response header is useless
        HeaderValueGuardClauses.StringCannotBeNullOrWhitsSpace(ReportUri, nameof(ReportUri));
        return base.BuildHeaderValue();
    }
#pragma warning restore CS0618
}
