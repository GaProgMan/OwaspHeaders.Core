namespace OwaspHeaders.Core.Models;

/// <summary>
/// Cross-Origin-Resource-Policy
/// This response header(also named CORP) allows to define a policy that let
/// websites and applications opt in to protection against certain requests
/// from other origins(such as those issued with elements like the script and
/// img tags), to mitigate speculative side-channel attacks, like Spectre, as
/// well as Cross-Site Script Inclusion(XSSI) attacks(source Mozilla MDN).
/// </summary>
public class CrossOriginResourcePolicy : IConfigurationBase
{
    /// <summary>
    /// Cross-Origin-Resource-Policy
    /// This response header(also named CORP) allows to define a policy that let
    /// websites and applications opt in to protection against certain requests
    /// from other origins(such as those issued with elements like the script and
    /// img tags), to mitigate speculative side-channel attacks, like Spectre, as
    /// well as Cross-Site Script Inclusion(XSSI) attacks(source Mozilla MDN).
    /// </summary>
    public CrossOriginResourcePolicy(CrossOriginResourceOptions value =
        CrossOriginResourceOptions.SameOrigin)
    {
        OptionValue = value;
    }

    /// <summary>
    /// Only requests from the same Origin (i.e. scheme + host + port) can read the resource.
    /// </summary>
    private const string SameOriginValue = "same-origin";

    /// <summary>
    /// Only requests from the same Site can read the resource.
    /// </summary>
    private const string SameSiteValue = "same-site";

    /// <summary>
    /// Requests from any Origin (both same-site and cross-site) can read the resource. 
    /// Browsers are using this policy when an CORP header is not specified.
    /// </summary>
    private const string CrossOriginValue = "cross-origin";

    public enum CrossOriginResourceOptions
    {
        /// <summary>
        /// <see cref="SameOriginValue"/>
        /// </summary>
        SameOrigin,

        /// <summary>
        /// <see cref="SameSiteValue"/>
        /// </summary>
        SameSite,

        /// <summary>
        /// <see cref="CrossOriginValue"/>
        /// </summary>
        CrossOrigin
    };

    public CrossOriginResourceOptions OptionValue { get; }

    /// <summary>
    /// Builds the HTTP header value
    /// </summary>
    /// <returns>A string representing the HTTP header value</returns>
    public string BuildHeaderValue()
    {
        switch (OptionValue)
        {
            case CrossOriginResourceOptions.CrossOrigin:
                return CrossOriginValue;
            case CrossOriginResourceOptions.SameSite:
                return SameSiteValue;
            case CrossOriginResourceOptions.SameOrigin:
            default:
                return SameOriginValue;
        }
    }
}
