using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OwaspHeaders.Core.Helpers;

namespace OwaspHeaders.Core.Models
{
    public class PermittedCrossDomainPolicyConfiguration : IConfigurationBase
    {
        public string OptionValue { get; set; }

        public string BuildHeaderValue()
        {
            if (String.IsNullOrWhiteSpace(OptionValue))
            {
                ArgumentExceptionHelper.RaiseException(nameof(OptionValue));
            }
            var stringBuilder = new StringBuilder();
            stringBuilder.Append(OptionValue);
            return stringBuilder.ToString();
        }
    }
}