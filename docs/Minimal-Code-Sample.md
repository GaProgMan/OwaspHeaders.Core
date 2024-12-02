---
title: Minimal Code Sample
layout: page
nav_order: 5
---

# Tips For Creating A Minimal Code Sample

From time to time it might be helpful to create a minimal code sample. This will be useful when submitting PRs, writing
documentation, asking questions, or reporting bugs.

The fastest way to create a minimal code sample would be:

**Step 1**: Create new ASP .NET Core WebAPI project

**Step 2**: Add the `OwaspHeaders.Core` package

**Step 3**: Add the `SecureHeadersMiddleware` to the ASP .NET Core Middleware pipeline:

```csharp
// in Program.cs
app.UseSecureHeadersMiddleware();
```

**Step 4**: Progressively add smaller and smaller features until you hit on the bug, issue, or have added the new feature
(dependent on what the code sample is for).

**Step 5**: If small enough, add the code sample as a comment, issue or PR. If not, create a [GitHub Gist] for it, and
link to it in as a comment, issue or PR.

---

[GitHub Gist]: https://gist.github.com/starred