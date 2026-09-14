namespace OwaspHeaders.Core.Guards;

internal static class ObjectGuardClauses
{
    /// <summary>
    /// Throws an <see cref="ArgumentNullException"/> when <paramref name="obj"/> is null.
    /// </summary>
    /// <remarks>
    /// The parameter is nullable because receiving null is the whole point of a null guard, and
    /// <c>[NotNull]</c> tells the compiler that it is not null once this method returns. That only
    /// holds because <see cref="ArgumentExceptionHelper"/>'s throw helpers are marked
    /// <c>[DoesNotReturn]</c>.
    /// </remarks>
    internal static void ObjectCannotBeNull([NotNull] object? obj, string parameterName, string message)
    {
        if (obj is null)
        {
            ArgumentExceptionHelper.RaiseArgumentNullException(parameterName, message);
        }
    }

}
