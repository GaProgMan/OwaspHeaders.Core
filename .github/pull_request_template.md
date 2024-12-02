## Rationale for this PR

This PR adds the following header/fixes the following bug...

If the PR contains a code change, especially if it is either fixing a bug or adding a new header, please link to either the OWASP Secure Headers Project page for it or the [MDN](https://developer.mozilla.org/en-US/) page for it.

This PR closes # <- place the issue number directly after the `#` character.

The following is a minimal code sample for the new feature

```csharp
// in Program.cs
app.UseSecureHeadersMiddleware();
```

## PR Checklist

Feel free to either check the following items (by place an `x` inside of the square brackets) or by replacing the square brackets with a relevant emoji from the following list:

- :white_check_mark: to indicate that you have checked something off
- :negative_squared_cross_mark: to indicate that you haven't checked something off
- :question: to indicate that something might not be relevant (writing tests for documentation changes, for instance)

### Essential

These items are essential and must be completed for each commit. If they are not completed, the PR may not be accepted.

- [ ] I have added tests to the OwaspHeaders.Core.Tests project
- [ ] I have run the `dotnet-format` command and fixed any .editorconfig issues
- [ ] I have ensured that the code coverage has not dropped below 65%
- [ ] I have increased the version number in OwaspHeaders.Core.csproj (only relevant for code changes)

### Optional

- [ ] I have documented the new feature in the docs directory
- [ ] I have provided a code sample, showing how someone could use the new code

## Any Other Information

This section is optional, but it might be useful to list any other information you think is relevant.
