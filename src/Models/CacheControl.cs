using System.Collections.Generic;
using System.Text;
using OwaspHeaders.Core.Enums;
using OwaspHeaders.Core.Helpers;

namespace OwaspHeaders.Core.Models
{
    public class CacheControl : IConfigurationBase
    {   
        /// <summary>
        /// Whether all or part of the HTTP response message is intended for a single user and must 
        /// not be cached by a shared cache.
        /// </summary>
        public bool Private { get; set; }
        
        /// <summary>
        /// The maximum age, specified in seconds, that the HTTP client is willing to accept a response.
        /// </summary>
        public int MaxAge { get; set; }
        
        /// <summary>
        /// Protected constructor, we can no longer create instances of this
        /// class without using the public constructor
        /// </summary>
        protected CacheControl() { }

        public CacheControl(bool @private, int maxAge = 86400)
        {
            Private = @private;
            MaxAge = maxAge;
        }
        
        /// <summary>
        /// Builds the HTTP header value
        /// </summary>
        /// <returns>A string representing the HTTP header value</returns>
        public string BuildHeaderValue()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append("max-age=");
            stringBuilder.Append(MaxAge);
            if (Private)
            {
                stringBuilder.Append(", private");
            }

            return stringBuilder.ToString();
        }
    }
}