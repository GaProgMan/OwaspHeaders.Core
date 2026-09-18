namespace OwaspHeaders.Core.Models;

public class ReportingEndpointsPolicy : IConfigurationBase
{
    private readonly Dictionary<string, Uri> _endpoints;

    /// <summary>
    /// Initializes a new instance of the ReportingEndpointsPolicy class
    /// </summary>
    /// <param name="endpoints">
    /// The endpoint names and the absolute URLs reports should be sent to
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="endpoints"/>, or any URL within it, is null
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when an endpoint name is null, empty or whitespace
    /// </exception>
    public ReportingEndpointsPolicy(Dictionary<string, Uri> endpoints)
    {
        ObjectGuardClauses.ObjectCannotBeNull(endpoints, nameof(endpoints),
            $"{nameof(endpoints)} cannot be null");

        ValidateEndpoints(endpoints);

        // Copied rather than stored by reference, as ClearSiteDataPathConfiguration does, so that
        // changing the caller's dictionary afterwards cannot get round the validation above.
        _endpoints = new Dictionary<string, Uri>(endpoints);
    }

    /// <summary>
    /// Validates the supplied endpoints
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when validation fails</exception>
    private static void ValidateEndpoints(Dictionary<string, Uri> endpoints)
    {
        foreach (var kvp in endpoints)
        {
            if (string.IsNullOrWhiteSpace(kvp.Key))
            {
                ArgumentExceptionHelper.RaiseException("endpoint name");
            }

            ObjectGuardClauses.ObjectCannotBeNull(kvp.Value, "endpoint",
                $"Uri cannot be null for endpoint: {kvp.Key}");
        }
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
