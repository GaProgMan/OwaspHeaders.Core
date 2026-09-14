namespace OwaspHeaders.Core.Guards;

internal static class HeaderValueGuardClauses
{
    /// <summary>
    /// Throws an <see cref="ArgumentException"/> when <paramref name="value"/> is null, empty or
    /// whitespace.
    /// </summary>
    /// <remarks>
    /// The parameter is nullable because receiving null is the whole point of the guard, and
    /// <c>[NotNull]</c> tells the compiler that it is not null once this method returns.
    /// </remarks>
    internal static void StringCannotBeNullOrWhiteSpace([NotNull] string? value, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            ArgumentExceptionHelper.RaiseException(parameterName);
        }
    }
}
