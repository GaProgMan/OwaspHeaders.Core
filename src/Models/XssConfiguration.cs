namespace OwaspHeaders.Core.Models;

public class XssConfiguration : IConfigurationBase
{
    /// <summary>
    /// Builds the HTTP header value
    /// </summary>
    /// <returns>A string representing the HTTP header value</returns>
    /// <remarks>
    /// Due to X-XSS-Protection being deprecated, this method is hardcoded to return
    /// a string with a single character of 0. 
    /// </remarks>
    public string BuildHeaderValue()
    {
        return "0";
    }
}
