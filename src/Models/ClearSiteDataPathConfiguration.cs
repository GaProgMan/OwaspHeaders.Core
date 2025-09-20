using System.Collections.Frozen;

namespace OwaspHeaders.Core.Models;

/// <summary>
/// Configuration for path-specific Clear-Site-Data header settings
/// </summary>
public class ClearSiteDataPathConfiguration
{
    /// <summary>
    /// Path-specific Clear-Site-Data configurations
    /// </summary>
    public FrozenDictionary<string, ClearSiteDataConfiguration> PathConfigurations { get; init; }

    /// <summary>
    /// Default configuration to use when no path-specific configuration matches
    /// </summary>
    public ClearSiteDataConfiguration DefaultConfiguration { get; init; }

    /// <summary>
    /// Initializes a new instance of the ClearSiteDataPathConfiguration class
    /// </summary>
    /// <param name="pathConfigurations">Dictionary of path-specific configurations</param>
    /// <param name="defaultConfiguration">Default configuration for non-matching paths</param>
    /// <exception cref="ArgumentNullException">Thrown when pathConfigurations is null</exception>
    /// <exception cref="ArgumentException">Thrown when path keys are invalid</exception>
    public ClearSiteDataPathConfiguration(
        Dictionary<string, ClearSiteDataConfiguration> pathConfigurations,
        ClearSiteDataConfiguration defaultConfiguration = null)
    {
        ObjectGuardClauses.ObjectCannotBeNull(pathConfigurations, nameof(pathConfigurations),
            $"{nameof(pathConfigurations)} cannot be null");

        ValidatePathConfigurations(pathConfigurations);

        // Sort by path length (descending) for longest-match precedence
        var sortedPaths = new Dictionary<string, ClearSiteDataConfiguration>();
        var keysList = new List<string>(pathConfigurations.Keys);
        keysList.Sort((a, b) => b.Length.CompareTo(a.Length)); // Descending order

        foreach (var key in keysList)
        {
            sortedPaths[key] = pathConfigurations[key];
        }

        PathConfigurations = sortedPaths.ToFrozenDictionary();
        DefaultConfiguration = defaultConfiguration;
    }

    /// <summary>
    /// Gets the configuration for the specified request path
    /// </summary>
    /// <param name="requestPath">The request path to match</param>
    /// <returns>The matching configuration or null if no match found</returns>
    public ClearSiteDataConfiguration GetConfigurationForPath(string requestPath)
    {
        if (string.IsNullOrEmpty(requestPath))
        {
            return DefaultConfiguration;
        }

        // Find exact path match (longest-match precedence due to sorting)
        foreach (var kvp in PathConfigurations)
        {
            if (string.Equals(requestPath, kvp.Key, StringComparison.InvariantCulture))
            {
                return kvp.Value;
            }
        }

        return DefaultConfiguration;
    }

    /// <summary>
    /// Validates the path configurations
    /// </summary>
    /// <param name="pathConfigurations">The path configurations to validate</param>
    /// <exception cref="ArgumentException">Thrown when validation fails</exception>
    private static void ValidatePathConfigurations(Dictionary<string, ClearSiteDataConfiguration> pathConfigurations)
    {
        foreach (var kvp in pathConfigurations)
        {
            if (string.IsNullOrWhiteSpace(kvp.Key))
            {
                ArgumentExceptionHelper.RaiseException("path");
            }

            ObjectGuardClauses.ObjectCannotBeNull(kvp.Value, "configuration",
                $"Configuration cannot be null for path: {kvp.Key}");
        }
    }
}
