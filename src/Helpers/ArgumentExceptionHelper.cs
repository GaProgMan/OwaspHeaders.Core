namespace OwaspHeaders.Core.Helpers;

/// <remarks>
/// Every method here is marked <c>[DoesNotReturn]</c>. Without that, a guard clause which marks
/// its checked parameter <c>[NotNull]</c> fails to compile with CS8777, because the compiler
/// cannot see that these helpers always throw.
/// </remarks>
internal static class ArgumentExceptionHelper
{
    /// <summary>
    /// Used to raise an <see cref="System.ArgumentException"/> whenever an argument is not supplied to a method
    /// </summary>
    [DoesNotReturn]
    internal static void RaiseException(string argumentName)
    {
        throw new ArgumentException($"No value for {argumentName} was supplied");
    }

    /// <summary>
    /// Used to raise an <see cref="System.ArgumentException"/> whenever a bool argument should be true, but is not
    /// </summary>
    [DoesNotReturn]
    internal static void RaiseNotTrueException(string argumentName)
    {
        throw new ArgumentException($"Value for {argumentName} must be true");
    }

    [DoesNotReturn]
    internal static void RaiseArgumentNullException(string argumentName, string message)
    {
        throw new ArgumentNullException(argumentName, message);
    }
}
