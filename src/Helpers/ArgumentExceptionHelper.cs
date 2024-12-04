namespace OwaspHeaders.Core.Helpers;

public static class ArgumentExceptionHelper
{
    /// <summary>
    /// Used to raise an <see cref="System.ArgumentException"/> whenever an argument is not supplied to a method
    /// </summary>
    public static void RaiseException(string argumentName)
    {
        throw new ArgumentException($"No value for {argumentName} was supplied");
    }

    public static void RaiseArgumentNullException(string argumentName, string message)
    {
        throw new ArgumentNullException(argumentName, message);
    }
}
