namespace OwaspHeaders.Core.Models;

public class ReportingEndpointsPolicy : IConfigurationBase
{
    public Dictionary<string, Uri> Endpoints = new();

    /// <summary>
    /// Protected constructor, we can no longer create instances of this class without
    /// using the public constructor
    /// </summary>
    [ExcludeFromCodeCoverage]
    protected ReportingEndpointsPolicy() { }

    public ReportingEndpointsPolicy(Dictionary<string, Uri> endpoints)
    {
        Endpoints = endpoints;
    }

    public string BuildHeaderValue()
    {
        if (Endpoints.Count != 0)
        {
            var stringBuilder = new StringBuilder();

            foreach (var kvp in Endpoints)
            {
                stringBuilder.Append($"{kvp.Key}=\"{kvp.Value}\", ");
            }

            return $"Reporting-Endpoints: {stringBuilder.TrimEnd().RemoveTrailingCharacter(',')}";
        }

        return string.Empty;
    }
}
