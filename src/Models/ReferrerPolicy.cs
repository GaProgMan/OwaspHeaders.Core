using OwaspHeaders.Core.Helpers;

namespace OwaspHeaders.Core.Models
{
    public class ReferrerPolicy : IConfigurationBase
    {
        public string OptionValue { get; set; }

        public string BuildHeaderValue()
        {
            if (string.IsNullOrWhiteSpace(OptionValue))
            {
                ArgumentExceptionHelper.RaiseException(nameof(OptionValue));
            }

            return OptionValue;
        }
    }
}