namespace OwaspHeaders.Core.Models;

public class ContentSecurityPolicySandBox : IConfigurationBase
{
    /// <summary>
    /// Protected constructor, we can no longer create instances of this class without
    /// using the public constructor
    /// </summary>
    [ExcludeFromCodeCoverage]
    protected ContentSecurityPolicySandBox() { }

    public IList<CspSandboxType> SandboxTypes { get; }

    public ContentSecurityPolicySandBox(params CspSandboxType[] sandboxType)
    {
        SandboxTypes = sandboxType;
    }

    public string BuildHeaderValue()
    {
        var returnStr = new StringBuilder("sandbox");
        foreach (var sandboxType in SandboxTypes)
        {
            switch (sandboxType)
            {
                case CspSandboxType.allowForms:
                    returnStr.Append(" allow-forms");
                    break;
                case CspSandboxType.allowModals:
                    returnStr.Append(" allow-modals");
                    break;
                case CspSandboxType.allowOrientationLock:
                    returnStr.Append(" allow-orientation-lock");
                    break;
                case CspSandboxType.allowPointerLock:
                    returnStr.Append(" allow-pointer-lock");
                    break;
                case CspSandboxType.allowPopups:
                    returnStr.Append(" allow-popups");
                    break;
                case CspSandboxType.allowPopupsToEscapeSandbox:
                    returnStr.Append(" allow-popups-to-escape-sandbox");
                    break;
                case CspSandboxType.allowPresentation:
                    returnStr.Append(" allow-presentation");
                    break;
                case CspSandboxType.allowSameOrigin:
                    returnStr.Append(" allow-same-origin");
                    break;
                case CspSandboxType.allowScripts:
                    returnStr.Append(" allow-scripts");
                    break;
                case CspSandboxType.allowTopNavigation:
                    returnStr.Append(" allow-top-navigation");
                    break;
            }
        }

        returnStr.Append(';');
        return returnStr.ToString();
    }
}
