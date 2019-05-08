using System.Collections.Generic;
using OwaspHeaders.Core.Enums;
using OwaspHeaders.Core.Helpers;

namespace OwaspHeaders.Core.Models
{
    public static class ContentSecurityPolicyExtensions
    {
        /// <summary>
        /// Used to set the Content Security Policy URIs for a given <see cref="CspUriType"/>
        /// </summary>
        public static ContentSecurityPolicyConfiguration SetCspUri
            (this ContentSecurityPolicyConfiguration @this, List<ContentSecurityPolicyElement> uris,
            CspUriType uriType)
        {
            switch (uriType)
            {
                case CspUriType.Base:
                    @this.BaseUri = uris;
                    break;
                case CspUriType.DefaultUri:
                    @this.DefaultSrc = uris;
                    break;
                case CspUriType.Script:
                    @this.ScriptSrc = uris;
                    break;
                case CspUriType.Object:
                    @this.ObjectSrc = uris;
                    break;
                case CspUriType.Style:
                    @this.StyleSrc = uris;
                    break;
                case CspUriType.Img:
                    @this.ImgSrc = uris;
                    break;
                case CspUriType.Media:
                    @this.MediaSrc = uris;
                    break;
                case CspUriType.Frame:
                    @this.FrameSrc = uris;
                    break;
                case CspUriType.Child:
                    @this.ChildSrc = uris;
                    break;
                case CspUriType.FrameAncestors:
                    @this.FrameAncestors = uris;
                    break;
                case CspUriType.Font:
                    @this.FontSrc = uris;
                    break;
                case CspUriType.Connect:
                    @this.ConnectSrc = uris;
                    break;
                case CspUriType.Manifest:
                    @this.ManifestSrc = uris;
                    break;
                case CspUriType.Form:
                    @this.FormAction = uris;
                    break;
                default:
                    ArgumentExceptionHelper.RaiseException(nameof(CspUriType));
                    break;
            }

            return @this;
        }

        /// <summary>
        /// Used the set the <see cref="CspSandboxType"/> forthe Content Secuity Policy
        /// </summary>
        public static ContentSecurityPolicyConfiguration SetSandbox
            (this ContentSecurityPolicyConfiguration @this, CspSandboxType sandboxType)
        {
            @this.Sandbox = new ContentSecurityPolicySandBox(sandboxType);

            return @this;
        }
    }
}