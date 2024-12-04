namespace OwaspHeaders.Core.Guards;

public static class HeaderValueGuardClauses
{
    public static void StringCannotBeNullOrWhitsSpace(string value, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            ArgumentExceptionHelper.RaiseException(parameterName);
        }
    }
}