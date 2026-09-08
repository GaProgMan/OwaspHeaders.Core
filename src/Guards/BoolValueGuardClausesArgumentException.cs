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
[Obsolete("BoolValueGuardClauses is no longer used by OwaspHeaders.Core and will be removed in " +
          "version 12. The Cross-Origin-Embedder-Policy pairing rule it enforced is now reported " +
          "by SecureHeadersMiddlewareConfiguration.Validate(). If you were calling this directly, " +
          "use your own guard clause or ArgumentOutOfRangeException.ThrowIfNotEqual instead. See " +
          "https://github.com/GaProgMan/OwaspHeaders.Core/issues/59", false)]
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
