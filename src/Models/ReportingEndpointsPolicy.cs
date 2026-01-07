namespace OwaspHeaders.Core.Models;

public class ReportingEndpointsPolicy : IConfigurationBase
{
    private readonly Dictionary<string, Uri> _endpoints = new();

    /// <summary>
    /// Protected constructor, we can no longer create instances of this class without
    /// using the public constructor
    /// </summary>
    [ExcludeFromCodeCoverage]
    protected ReportingEndpointsPolicy() { }

    public ReportingEndpointsPolicy(Dictionary<string, Uri> endpoints)
    {
        _endpoints = endpoints;
    }

    public string BuildHeaderValue()
    {
        if (_endpoints.Count == 0)
        {
            return string.Empty;
        }

        var stringBuilder = new StringBuilder();

        foreach (var kvp in _endpoints)
        {
            stringBuilder.Append($"{kvp.Key}=\"{kvp.Value}\", ");
        }

        return stringBuilder.TrimEnd().RemoveTrailingCharacter(',').ToString();
    }
}
