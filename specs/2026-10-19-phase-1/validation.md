# Phase 1 — Validation

The PR for `feature/phase-1` can merge when every check below passes and the evidence section is filled in.

## Roadmap "Done when"

- [ ] `dotnet test` runs from the repository root.
- [ ] One deliberately failing test was **seen to fail in CI**: the PR has a red `build-test-lint` run that failed at the test step.
- [ ] The same test was then **made to pass**: a later `build-test-lint` run on the PR is green.

## Structure

- [ ] `Kotsu.slnx` is at the root and references exactly `backend/Kotsu.Scheduling` and `backend/tests/Kotsu.Scheduling.Tests`.
- [ ] `Kotsu.Scheduling` has **no** `PackageReference` and **no** `ProjectReference` (pure library, see tech-stack.md).
- [ ] `Kotsu.Scheduling.Tests` uses xUnit and references `Kotsu.Scheduling`.
- [ ] Every test follows Arrange, Act, Assert: the sections are marked with `// Arrange`, `// Act` and `// Assert` comments, in that order, with one Act per test (tech-stack.md). Checked in code review.
- [ ] Both projects build for `net11.0`. The target framework comes from `Directory.Build.props`, not from each csproj.
- [ ] `global.json` pins `11.0.100-rc.2.26504.105`, and `dotnet --version` at the root matches it.
- [ ] `.gitignore`, `.gitattributes` and `.editorconfig` are present. No `bin/`, `obj/` or `TestResults/` are committed.

## Build and lint

Run from the repository root on a clean clone:

- [ ] `dotnet restore` succeeds.
- [ ] `dotnet build --no-restore` succeeds with **0 warnings, 0 errors**.
- [ ] `dotnet format --verify-no-changes` exits 0.
- [ ] `dotnet test --no-build` reports 1 passed, 0 failed.
- [ ] A deliberate formatting violation makes `dotnet format --verify-no-changes` exit non-zero (checked locally, not committed).
- [ ] A deliberate analyzer violation makes `dotnet build` fail (checked locally, not committed).

## CI

- [ ] `.github/workflows/ci.yml` runs on `pull_request` to `main` and `push` to `main`.
- [ ] The job is named `build-test-lint` and runs restore, build, format verification and test, in that order.
- [ ] The SDK is installed from `global.json`, so CI and local use the same SDK.
- [ ] Test results are uploaded as a workflow artifact.

## Merge gate

- [ ] Branch protection on `main` requires the `build-test-lint` check, an up-to-date branch and a pull request.
- [ ] The PR page shows `build-test-lint` as **Required**.

## Documentation

- [ ] tech-stack.md's Runtime row says .NET 11 (Decided), and the repository layout shows the root build files.
- [ ] README status and tech stack reflect the scaffold, with instructions to build and test.

## Evidence

Fill this in before you mark the PR ready.

| Item | Link or output |
| --- | --- |
| Red CI run (failing test) | https://github.com/chinthaka/kotsu/actions/runs/39276487572 (restore, build and lint passed; failed at Test) |
| Green CI run | https://github.com/chinthaka/kotsu/actions/runs/39276706177 (all steps passed) |
| Local `dotnet test`, red (summary line) | `Failed!  - Failed: 1, Passed: 0, Skipped: 0, Total: 1 - Kotsu.Scheduling.Tests.dll (net11.0)` |
| Local `dotnet test`, green (summary line) | `Passed!  - Failed: 0, Passed: 1, Skipped: 0, Total: 1 - Kotsu.Scheduling.Tests.dll (net11.0)` |
| Branch protection (screenshot or `gh api` output) | |
