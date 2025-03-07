﻿namespace OwaspHeaders.Core.Enums;

/// <remarks>Please note: these enum values are named after Referrer Policy Options
/// exactly. This is so that we can use the value as a string, without having to
/// do any C# string magic (and waste cycles doing so) to get the right names. 
/// This does mean that Rider (et al.) will tell you that the naming convention
/// here is non-standard.</remarks>
public enum ReferrerPolicyOptions
{
    noReferrer,
    noReferrerWhenDowngrade,
    origin,
    originWhenCrossOrigin,
    sameOrigin,
    strictOrigin,
    strictWhenCrossOrigin,
    unsafeUrl
};
