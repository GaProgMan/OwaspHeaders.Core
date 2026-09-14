namespace OwaspHeaders.Core.Guards;

/// <summary>
/// Guard clause which throws when a boolean argument is not true.
/// </summary>
/// <remarks>
/// This has no callers left inside the library. Its only use was enforcing the
/// Cross-Origin-Embedder-Policy / Cross-Origin-Resource-Policy pairing rule during header
/// generation; that rule is now reported by
/// <see cref="SecureHeadersMiddlewareConfiguration.Validate"/> instead, so it is caught as the
/// request pipeline is built rather than on the first request.
/// </remarks>
/// <remarks>
/// It was public and <c>[Obsolete]</c> earlier in version 11, for removal in version 12. It is
/// now internal along with the other guard clauses, so the deprecation notice — which addressed
/// consumers who can no longer reach it — has gone. Nothing calls it, so it can simply be deleted
/// in version 12. Its tests are what keep it honest until then.
/// </remarks>
internal static class BoolValueGuardClauses
{
    internal static void MustBeTrue(bool value, string parameterName)
    {
        if (!value)
        {
            ArgumentExceptionHelper.RaiseNotTrueException(parameterName);
        }
    }
}
