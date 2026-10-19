# Phase 1 — Requirements

Roadmap phase 1 ([roadmap.md](../roadmap.md)), Stage 1: Scheduling engine.

## Goal

Lay down the .NET solution, the empty `Kotsu.Scheduling` library and its test project, and a GitHub Actions workflow that tests and lints every pull request. Phases 2–12 build the engine test-first on top of this.

**Done when** (from the roadmap): `dotnet test` runs, and one deliberately failing test is seen to fail and is then made to pass.

## Context

- The GitHub repository already exists (`github.com/chinthaka/kotsu`) and holds only `specs/`, `README.md` and `LICENSE`. So "create the GitHub repository" is already done. This phase adds everything else.
- [tech-stack.md](../tech-stack.md) says the scheduling engine is a **pure C# class library with no I/O, database or web dependencies**, tested with **xUnit**, built **test-first**, and that **tests and lint run in CI and block the merge**.
- CI config and build scripts are TDD exceptions (tech-stack.md, "Development practice"). The failing-then-passing test is the TDD proof for this phase.

## Scope

In scope:

1. A solution file at the repository root that references the engine and test projects.
2. `backend/Kotsu.Scheduling/`: an empty class library. It has no package references and no project references.
3. `backend/tests/Kotsu.Scheduling.Tests/`: an xUnit test project that references `Kotsu.Scheduling`.
4. Shared build settings: `global.json`, `Directory.Build.props` and `.editorconfig` at the repository root.
5. `.github/workflows/ci.yml`: restore, build, lint and test on every pull request to `main` and on every push to `main`.
6. Branch protection on `main` that requires the CI checks to pass.
7. A `.gitignore` for .NET build output.
8. Updates to `tech-stack.md` and `README.md` for the runtime decision below.

Out of scope:

- Any domain types or scheduling logic. These start in phase 2.
- `Kotsu.Api`, `Kotsu.Data`, the clients, the infrastructure and deployment. These start in phase 16 or later.
- Code coverage thresholds, release builds and packaging.

## Decisions

| # | Decision | Notes |
| --- | --- | --- |
| D1 | **Target .NET 11** (`net11.0`), ahead of tech-stack.md's .NET 10. | This was chosen in the phase 1 review. .NET 11 is at RC 2 (`11.0.100-rc.2.26504.105` is installed locally), and GA is expected in November 2026. tech-stack.md already says "Move to each new release as it ships", so this phase starts on 11 rather than migrating later. |
| D2 | `global.json` pins the SDK to `11.0.100-rc.2.26504.105` with `rollForward: latestFeature` and `allowPrerelease: true`. | CI installs the SDK from `global.json` (`actions/setup-dotnet` with `global-json-file`), so local and CI builds use the same SDK. When .NET 11 reaches GA, `global.json` moves to the GA SDK and `allowPrerelease` is removed (follow-up, not in this phase). |
| D3 | tech-stack.md's Runtime row changes to ".NET 11, C#". | This keeps the constitution true. Record it as Decided. |
| D4 | **Lint = `dotnet format --verify-no-changes` plus the built-in .NET analyzers.** | `.editorconfig` defines the style. `Directory.Build.props` sets `EnableNETAnalyzers`, `AnalysisLevel=latest-recommended`, `EnforceCodeStyleInBuild`, `TreatWarningsAsErrors`, `Nullable=enable` and `ImplicitUsings=enable`. It also sets `GenerateDocumentationFile`, which IDE0005 (unused usings) needs to run in the build, and suppresses CS1591, so XML doc comments are not required. Analyzer warnings therefore fail the build, and formatting drift fails the lint step. StyleCop is not used. |
| D5 | The solution file is at the repository root (`Kotsu.slnx`, the SDK's default XML solution format). | `dotnet build`, `dotnet test` and `dotnet format` can then run from the root with no path. Later backend projects (`Kotsu.Api`, `Kotsu.Data`) join the same solution. |
| D6 | Layout follows tech-stack.md: `backend/Kotsu.Scheduling/` and `backend/tests/Kotsu.Scheduling.Tests/`. | |
| D7 | The test framework is xUnit, using the template that ships with the .NET 11 SDK (`dotnet new xunit`). | tech-stack.md: Decided. |
| D8 | The CI workflow runs on `ubuntu-latest` and has a single job named `build-test-lint`. | That job name is the required status check in branch protection. |
| D9 | The failing-test proof must show in CI, not only locally. | A commit with a deliberately failing test is pushed, so the PR's CI run goes red. A follow-up commit makes it pass, so the run goes green. Both runs stay visible in the PR's checks history. |
| D10 | Branch protection on `main` requires `build-test-lint` to pass and the branch to be up to date before merging. | This puts tech-stack.md's "a pull request cannot merge while either fails" into effect. |

## Constraints

- `Kotsu.Scheduling` must stay free of I/O, database and web dependencies. In this phase it has zero package references.
- No KS-specific or country-specific types. None are expected here, but nothing may be added that later phases would need to undo.
- The build must give zero warnings, because `TreatWarningsAsErrors` is on.
- Every test follows Arrange, Act, Assert, with the sections marked by comments (tech-stack.md, "Development practice"). The scaffold test is the first example of this.

## Risks

- **Preview SDK on CI runners.** `setup-dotnet` must be able to fetch the RC 2 SDK. If it can't, use `dotnet-quality: preview` with `dotnet-version: 11.0.x`.
- **RC to GA changes.** Analyzer rules or the templates may change at GA. Re-running lint after the GA bump is part of that follow-up.
- **Branch protection needs repository admin rights.** If `gh api` can't set it, the owner sets it by hand in GitHub settings, and the validation step checks the result.
