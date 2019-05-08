using OwaspHeaders.Core.Enums;

namespace OwaspHeaders.Core.Models
{
    public class ContentSecurityPolicySandBox : IConfigurationBase
    {
        protected ContentSecurityPolicySandBox() {}

        public CspSandboxType SandboxType { get; set; }
        
        public ContentSecurityPolicySandBox(CspSandboxType sandboxType)
        {
            SandboxType = sandboxType;
        }

        public string BuildHeaderValue()
        {
            var returnStr = "sandbox";
            switch (SandboxType)
            {
                case CspSandboxType.allowForms:
                    return $"{returnStr} allow-forms; ";
                case CspSandboxType.allowModals:
                    return $"{returnStr} allow-modals; ";
                case CspSandboxType.allowOrientationLock:
                    return $"{returnStr} allow-orientation-lock; ";
                case CspSandboxType.allowPointerLock:
                    return $"{returnStr} allow-pointer-lock; ";
                case CspSandboxType.allowPopups:
                    return $"{returnStr} allow-popups; ";
                case CspSandboxType.allowPopupsToEscapeSandbox:
                    return $"{returnStr} allow-popups-to-escape-sandbox; ";
                case CspSandboxType.allowPresentation:
                    return $"{returnStr} allow-presentation; ";
                case CspSandboxType.allowSameOrigin:
                    return $"{returnStr} allow-same-origin; ";
                case CspSandboxType.allowScripts:
                    return $"{returnStr} allow-scripts; ";
                case CspSandboxType.allowTopNavigation:
                    return $"{returnStr} allow-top-navigation; ";
            }

            return $"{returnStr}; ";
        }
    }
}