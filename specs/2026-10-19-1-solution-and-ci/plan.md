# Phase 1 — Plan

Branch: `feature/phase-1`. See [requirement.md](requirement.md) for the decisions (D1–D10) and [validation.md](validation.md) for the merge criteria.

Each section ends with something that can be checked. Work through them in order.

## 1. Repository basics

1.1 Add a `.gitignore` for .NET (`bin/`, `obj/`, `*.user`, `.vs/`, `TestResults/`, and so on).
1.2 Add `global.json` that pins SDK `11.0.100-rc.2.26504.105` with `rollForward: latestFeature` and `allowPrerelease: true` (D2).
1.3 Check: `dotnet --version` at the repository root prints the pinned SDK.

## 2. Shared build settings

2.1 Add `Directory.Build.props` at the root with `TargetFramework=net11.0`, `Nullable=enable`, `ImplicitUsings=enable`, `LangVersion=latest`, `EnableNETAnalyzers=true`, `AnalysisLevel=latest-recommended`, `EnforceCodeStyleInBuild=true` and `TreatWarningsAsErrors=true` (D1, D4).
2.2 Add `.editorconfig` at the root: indentation, line endings (`lf`), final newline, C# naming and style rules, and severities chosen so the style rules are enforced in the build.
2.3 Add `.gitattributes` with `* text=auto eol=lf`, so the Windows checkout and the Linux CI agree on line endings for `dotnet format`.

## 3. Solution and projects

3.1 `dotnet new sln -n Kotsu` at the root, which creates `Kotsu.slnx` (D5).
3.2 `dotnet new classlib -o backend/Kotsu.Scheduling`. Remove the template's `Class1.cs`, and remove `TargetFramework` from the csproj because it comes from `Directory.Build.props`. Leave a placeholder so the assembly builds; the namespace root is `Kotsu.Scheduling`.
3.3 `dotnet new xunit -o backend/tests/Kotsu.Scheduling.Tests`. Add a project reference to `Kotsu.Scheduling`, and remove the duplicated `TargetFramework`.
3.4 Add both projects to `Kotsu.slnx`.
3.5 Check: `dotnet build` at the root succeeds with zero warnings.

## 4. Red: the deliberately failing test

4.1 Add one test to `Kotsu.Scheduling.Tests` that calls a stub member in `Kotsu.Scheduling` and asserts a value the stub does not yet return. An example is a `ScaffoldCheck.IsReady()` that returns `false`, asserted to be `true`. The test exercises the project reference as well as the test runner. Structure it as Arrange, Act, Assert, with `// Arrange`, `// Act` and `// Assert` comments (tech-stack.md, "Development practice"), so later phases have an example to copy.
4.2 Run `dotnet test` locally and confirm one test fails. Save the output summary for validation.md.
4.3 Commit as "Phase 1: add deliberately failing scaffold test (red)".

## 5. CI workflow

5.1 Add `.github/workflows/ci.yml` with a job named `build-test-lint` (D8):
   - Triggers: `pull_request` to `main`, `push` to `main`.
   - `actions/checkout`, then `actions/setup-dotnet` with `global-json-file: global.json`.
   - `dotnet restore`
   - `dotnet build --no-restore` (the analyzers run here, and warnings are errors)
   - `dotnet format --verify-no-changes --no-restore` (lint)
   - `dotnet test --no-build`, with test results uploaded as an artifact.
5.2 Run `dotnet format --verify-no-changes` locally to make sure it passes before pushing.
5.3 Commit, push the branch and open a draft PR to `main`.
5.4 Check: the PR's `build-test-lint` run **fails at the test step** (red), and build and lint pass.

## 6. Green: make it pass

6.1 Change the stub so the test passes, with the least code needed. For example, `IsReady()` returns `true`. Leave the test itself unchanged, including its Arrange, Act and Assert structure.
6.2 Run `dotnet test` locally and confirm it passes. Save the output summary.
6.3 Commit as "Phase 1: make scaffold test pass (green)" and push.
6.4 Check: the PR's `build-test-lint` run is green.

## 7. Lint gate check

7.1 On a throwaway local change (not committed), break the formatting, for example with bad indentation, and confirm that `dotnet format --verify-no-changes` exits non-zero.
7.2 On a throwaway local change, add an analyzer violation, for example an unused variable or a nullable warning, and confirm that `dotnet build` fails.
7.3 Undo both changes.

## 8. Branch protection

8.1 Protect `main`: require the `build-test-lint` status check, require branches to be up to date, and require a pull request before merging (D10). Use `gh api repos/chinthaka/kotsu/branches/main/protection`, or the GitHub settings page if admin rights through `gh` are not available.
8.2 Check: the PR shows `build-test-lint` as a required check.

## 9. Documentation

9.1 Update `specs/tech-stack.md`: change the Runtime row to ".NET 11, C#" (D3), and add the solution and build files at the root to the repository layout block.
9.2 Update `README.md`: change the status line from "No application code has been written yet" to say that the engine scaffold exists, add how to build and test (`dotnet test` from the root), and add .NET 11 to the tech stack table.
9.3 Fill in the evidence section of [validation.md](validation.md) (red and green run links, local outputs).
9.4 Mark the PR ready for review.
