namespace OwaspHeaders.Core.Guards;

public static class BoolValueGuardClauses
{
    public static void MustBeTrue(bool value, string parameterName)
    {
        if (!value)
        {
            ArgumentExceptionHelper.RaiseNotTrueException(parameterName);
        }
    }
}
