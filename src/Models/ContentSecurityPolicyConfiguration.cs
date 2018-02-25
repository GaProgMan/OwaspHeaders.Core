using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using OwaspHeaders.Core.Extensions;

namespace OwaspHeaders.Core.Models
{
    public class ContentSecurityPolicyConfiguration : IConfigurationBase
    {
        /// <summary>
        /// The base-uri values to use (which can be used in a document's base element)
        /// </summary>
        public List<ContenSecurityPolicyElement> BaseUri { get; set; }

        /// <summary>
        /// The default-src values to use (as a fallback for the other CSP rules)
        /// </summary>
        public List<ContenSecurityPolicyElement> DefaultSrc { get; set; }

        /// <summary>
        /// The script-src values to use (valid sources for sources for JavaScript)
        /// </summary>
        public List<ContenSecurityPolicyElement> ScriptSrc { get; set; }

        /// <summary>
        /// The object-src values to use (valid sources for the object, embed, and applet elements)
        /// </summary>
        public List<ContenSecurityPolicyElement> ObjectSrc { get; set; }

        /// <summary>
        /// The style-src values to use (valid sources for style sheets)
        /// </summary>
        public List<ContenSecurityPolicyElement> StyleSrc { get; set; }

        /// <summary>
        /// The img-src values to use (valid sources for images and favicons)
        /// </summary>
        public List<ContenSecurityPolicyElement> ImgSrc { get; set; }

        /// <summary>
        /// The media-src values to use (valid sources for loading media using the audio and video elements)
        /// </summary>
        public List<ContenSecurityPolicyElement> MediaSrc { get; set; }

        /// <summary>
        /// The frame-src values to use (valid sources for nested browsing contexts loading using elements such as frame and iframe)
        /// </summary>
        public List<ContenSecurityPolicyElement> FrameSrc { get; set; }

        /// <summary>
        /// The child-src values to use (valid sources for web workers and nested browsing contexts loaded using elements such as frame and iframe)
        /// </summary>
        public List<ContenSecurityPolicyElement> ChildSrc { get; set; }

        /// <summary>
        /// The frame-ancestors values to use (valid parents that may embed a page using frame, iframe, object, embed, or applet)
        /// </summary>
        public List<ContenSecurityPolicyElement> FrameAncestors { get; set; }

        /// <summary>
        /// The font-src values to use (valid sources for fonts loaded using @font-face)
        /// </summary>
        public List<ContenSecurityPolicyElement> FontSrc { get; set; }

        /// <summary>
        /// The connect-src values to use (restricts the URLs which can be loaded using script interfaces)
        /// </summary>
        public List<ContenSecurityPolicyElement> ConnectSrc { get; set; }

        /// <summary>
        /// The manifest-src values to use (which manifest can be applied to the resource)
        /// </summary>
        public List<ContenSecurityPolicyElement> ManifestSrc { get; set; }

        /// <summary>
        /// The form-action values to use (restricts the URLs which can be used as the target of a form submissions from a given context)
        /// </summary>
        public List<ContenSecurityPolicyElement> FormAction { get; set; }
        
        /// <summary>
        /// Specifies an HTML sandbox policy that the user agent applies to the protected resource.
        /// </summary>
        public ContentSecurityPolicySandBox Sandbox { get; set; }
        
        /// <summary>
        /// Define the set of plugins that can be invoked by the protected resource by limiting the types of resources that can be embedded
        /// </summary>
        public string PluginTypes { get; set; }

        /// <summary>
        /// Whether to include the block-all-mixed-content directive (prevents loading any assets using HTTP when the page is loaded using HTTPS)
        /// </summary>
        public bool BlockAllMixedContent { get; set; }

        /// <summary>
        /// Whether to include the upgrade-insecure-requests directive (instructs user agents to treat all of a site's insecure URLs as though they have been replaced with secure URLs)
        /// </summary>
        public bool UpgradeInsecureRequests { get; set; }
        
        /// <summary>
        /// Define information user agent must send in Referer header
        /// </summary>
        public string Referrer { get; set; }

        /// <summary>
        /// Whether to instruct the user agent to report attempts to violate the Content Security Policy
        /// </summary>
        public string ReportUri { get; set; }

        protected ContentSecurityPolicyConfiguration() { }

        public ContentSecurityPolicyConfiguration(string pluginTypes, bool blockAllMixedContent,
            bool upgradeInsecureRequests, string referrer, string reportUri)
        {
            BaseUri = new List<ContenSecurityPolicyElement>();
            DefaultSrc = new List<ContenSecurityPolicyElement>();
            ScriptSrc = new List<ContenSecurityPolicyElement>();
            ObjectSrc = new List<ContenSecurityPolicyElement>();
            StyleSrc = new List<ContenSecurityPolicyElement>();
            ImgSrc = new List<ContenSecurityPolicyElement>();
            MediaSrc = new List<ContenSecurityPolicyElement>();
            FrameSrc = new List<ContenSecurityPolicyElement>();
            ChildSrc = new List<ContenSecurityPolicyElement>();
            FrameAncestors = new List<ContenSecurityPolicyElement>();
            FontSrc = new List<ContenSecurityPolicyElement>();
            ConnectSrc = new List<ContenSecurityPolicyElement>();
            ManifestSrc = new List<ContenSecurityPolicyElement>();
            FormAction = new List<ContenSecurityPolicyElement>();

            PluginTypes = pluginTypes;
            BlockAllMixedContent = blockAllMixedContent;
            UpgradeInsecureRequests = upgradeInsecureRequests;
            Referrer = referrer;
            ReportUri = reportUri;
        }

        /// <summary>
        /// Builds the HTTP header value
        /// </summary>
        /// <returns>A string representing the HTTP header value</returns>
        public string BuildHeaderValue()
        {
            var stringBuilder = new StringBuilder();

            if (AnyValues())
            {
                stringBuilder.BuildValuesForDirective("base-uri", BaseUri);
                stringBuilder.BuildValuesForDirective("default-src", DefaultSrc);
                stringBuilder.BuildValuesForDirective("script-src", ScriptSrc);
                stringBuilder.BuildValuesForDirective("object-src", ObjectSrc);
                stringBuilder.BuildValuesForDirective("style-src", StyleSrc);
                stringBuilder.BuildValuesForDirective("img-src", ImgSrc);
                stringBuilder.BuildValuesForDirective("media-src", MediaSrc);
                stringBuilder.BuildValuesForDirective("frame-src", FrameSrc);
                stringBuilder.BuildValuesForDirective("child-src", ChildSrc);
                stringBuilder.BuildValuesForDirective("frame-ancestors", FrameAncestors);
                stringBuilder.BuildValuesForDirective("font-src", FontSrc);
                stringBuilder.BuildValuesForDirective("connect-src", ConnectSrc);
                stringBuilder.BuildValuesForDirective("manifest-src", ManifestSrc);
                stringBuilder.BuildValuesForDirective("form-action", FormAction);
            }

            if (Sandbox != null)
            {
                stringBuilder.Append(Sandbox.BuildHeaderValue());
            }

            if (!string.IsNullOrWhiteSpace(PluginTypes))
            {
                stringBuilder.Append($"plugin-types {PluginTypes}; ");
            }

            if (BlockAllMixedContent)
            {
                stringBuilder.Append("block-all-mixed-content; ");
            }
            
            if (UpgradeInsecureRequests)
            {
                stringBuilder.Append("upgrade-insecure-requests; ");
            }

            if (!string.IsNullOrWhiteSpace(Referrer))
            {
                stringBuilder.Append($"referrer {Referrer}; ");
            }
            
            if (!string.IsNullOrWhiteSpace(ReportUri))
            {
                stringBuilder.Append($"report-uri {ReportUri}; ");
            }

            return stringBuilder.ToString().TrimEnd();
        }

        private bool AnyValues()
        {
            return BaseUri.Any()    || DefaultSrc.Any() || ScriptSrc.Any()  || ObjectSrc.Any()
                || StyleSrc.Any()   || ImgSrc.Any()     || MediaSrc.Any()   || FrameSrc.Any()
                || ChildSrc.Any()   || FrameAncestors.Any() || FontSrc.Any()|| ConnectSrc.Any()
                || ManifestSrc.Any()|| FormAction.Any();
        }
    }
}
