namespace OwaspHeaders.Core.Models
{
    /// <summary>
    /// Cross-Origin-Resource-Policy
    /// This response header(also named CORP) allows to define a policy that lets web sites and applications opt in to protection 
    /// against certain requests from other origins(such as those issued with elements like<script> and <img>), to mitigate speculative 
    /// side-channel attacks, like Spectre, as well as Cross-Site Script Inclusion(XSSI) attacks(source Mozilla MDN).
    /// </summary>
    public class CrossOriginResourcePolicy : IConfigurationBase
    {
        /// <summary>
        /// Only requests from the same Origin (i.e. scheme + host + port) can read the resource.
        /// </summary>
        public const string SameOriginValue = "same-origin";
        /// <summary>
        /// Only requests from the same Site can read the resource.
        /// </summary>
        public const string SameSiteValue = "same-site";
        /// <summary>
        /// Requests from any Origin (both same-site and cross-site) can read the resource. 
        /// Browsers are using this policy when an CORP header is not specified.
        /// </summary>
        public const string CrossOriginValue = "same-origin";

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

        public CrossOriginResourceOptions OptionValue { get; set; }

        public CrossOriginResourcePolicy(CrossOriginResourceOptions value = CrossOriginResourceOptions.SameOrigin)
        {
            OptionValue = value;
        }

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
}
