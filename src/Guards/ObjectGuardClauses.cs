namespace OwaspHeaders.Core.Guards;

public static class ObjectGuardClauses
{
    public static void ObjectCannotBeNull(Object obj, string parameterName, string message)
    {
        if (obj is null)
        {
            ArgumentExceptionHelper.RaiseArgumentNullException(parameterName, message);
        }
    }
    
}
