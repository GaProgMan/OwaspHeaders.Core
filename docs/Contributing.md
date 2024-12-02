---
title: Contributing
layout: page
nav_order: 3
---

# Contributing to OwaspHeaders.Core

This project is completely open-source, and as such we greatly appreciate any contributions to the project. Without the
open-source community contributions, progress on this project would be rather slow.

### Code of Conduct

We expect all active community members to adhere to our [Code of Conduct] and any violations of that code of conduct
will be taken very seriously. It, essentially boils down to the following two words: be nice.

### Pull Requests

As is traditional in the open-source sphere, we would prefer that any pull requests are attached to an issue. If there
isn't an issue which fits the feature you would like to implement, please consider creating an issue beforehand. That
way we can track a pull request back to the issue&mdash;and the conversations around it&mdash;which spawned it.

We use GitHub to track all issues and welcome all community input on them, so please raise an issue on the GitHub
repository for the project [Issues].

Most of the pull requests we would love to see are for implementing the HTTP headers that we have missed from the
[OWASP Secure Headers Project]. As such we will prioritise accepting PRs for missing headers, bug fixes, or documentation.

#### Process

In order to submit a PR to OwaspHeaders.Core, please follow these steps (or similar):

1. Fork the project on GitHub
2. Clone your forked version of the repository to your development machine
3. Create a branch with a descriptive name under the `feature/` directory
4. Commit all of your changes to that branch
5. Don't forget to add tests 
6. Ensure that you run `dotnet-format` (with the optional `fix` verb for auto-fixes)
7. Push those changes to your forked version of the repository 
8. Create a Pull Request through the GitHub user interface and leave a detailed description

For instance, if you were to create a branch to implement a new HTTP header called `X-Example-Header`:

1. Fork the project on GitHub
2. Clone your forked version of the repository to your development machine
3. Create a branch called `feature/x-example-header` (you might do this with `git checkout -b feature/x-example-header`)
4. Add code changes
5. Add tests
6. Run `dotnet-format fix` to ensure that all formatting is correct
7. Push all the changes to the fork of the repository
8. Create a Pull Request with the following information

{: .quote }
> ### Rationale for this PR
> 
> This PR adds the `X-Example-Header` HTTP header, which you can read about on the [OWASP Secure Headers Project](). 
> This PR closes #999
>
> ### PR Checklist
>
> Feel free to either check the following items (by place an `x` inside of the square brackets) or by replacing the square brackets with a relevant emoji from the following list:
> 
> - :white_check_mark: to indicate that you have checked something off
> - :negative_squared_cross_mark: to indicate that you haven't checked something off
> - :question: to indicate that something might not be relevant (writing tests for documentation changes, for instance)
> 
> #### Essential
> 
> These items are essential and must be completed for each commit. If they are not completed, the PR may not be accepted.
> 
> - [x] I have added tests to the OwaspHeaders.Core.Tests project
> - [x] I have run the `dotnet-format` command and fixed any .editorconfig issues
> - [x] I have ensured that the code coverage has not dropped below 65%
> - [x] I have increased the version number in OwaspHeaders.Core.csproj (only relevant for code changes)
> #### Optional
> 
> - :negative_squared_cross_mark: I have documented the new feature in the docs directory
> 
> ### Any Other Information
> 
> No other information

Once the PR has been raised, one of the core contributors to the project will review the PR. They will add comments using
the following [Code Review Guide] written by GitHub user erikthedev. This ensures that everyone understands the meanings
behind the comments and that there can be no misunderstandings around tone of voice used in the comments. 

There are a number of automatic checks which take place on PRs, they will likely run before a core contributor will be
able to comment on your PR. These checks consist of running all the tests, collecting test coverage data, and ensuring
that the code adheres to the coding standards set out in the .editorconfig file. Failing one of these automatic steps
will hold all PR comments until you push changes which make them pass.

Please don't hesitate to ask for help in the PR comments, should you need it.

----

[Issues]: https://github.com/GaProgMan/OwaspHeaders.Core/issues
[OWASP Secure Headers Project]: https://owasp.org/www-project-secure-headers/#div-headers
[Code of Conduct]: https://gaprogman.github.io/OwaspHeaders.Core/Code-of-Conduct
[Code Review Guide]: https://github.com/erikthedeveloper/code-review-emoji-guide