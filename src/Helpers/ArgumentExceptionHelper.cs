namespace OwaspHeaders.Core.Helpers
{
    public static class ArgumentExceptionHelper
    {
        public static void RaiseException(string argumentName)
        {
            throw new System.ArgumentException($"No value for {argumentName} was supplied");
        }
    }
}