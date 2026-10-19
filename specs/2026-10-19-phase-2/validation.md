# Phase 2 — Validation

The PR for `feature/phase-2` can merge when every check below passes and the evidence section is filled in.

## Roadmap "Done when"

- [ ] All the types exist: country, qualification stage, subject, exam board, syllabus topic, sub-topic (with estimated study hours), schedule window, weekly study availability (7 days) and progress.
- [ ] Every validation rule in [requirement.md](requirement.md) (D3–D13) has at least one test that fails when the rule is broken.
- [ ] The qualification stage is generic content per country. The Great Britain fixture test builds England, Scotland and Wales stages purely as data, including SQA National 5, Higher and Advanced Higher.

## Design

- [ ] Every validated type is created only through `Create(...)`, which returns `Result<T>`. No public constructor accepts unchecked input (D1).
- [ ] `Create` doesn't throw for invalid input, and it reports every broken rule, each with a stable `Code` (D2).
- [ ] `StudyHours` uses `decimal` and accepts only multiples of 0.5 that are zero or more (D3). A sub-topic needs at least 0.5 hours, and a day takes 0–24 hours (D4).
- [ ] Ids are typed `readonly record struct`s with code rules (D5), and a `StageId` can't be passed where a `SubjectId` is expected (this is a compile-time guarantee, so no test is needed).
- [ ] `Country.RequiresBoardChoice` is true for England and false for Scotland and Wales in the fixture (D7).
- [ ] A topic's hours are derived from its sub-topics (D9).
- [ ] `ScheduleWindow.EndDate` is inclusive and correct across month and year boundaries (D11).
- [ ] An all-zero `WeeklyAvailability` is valid (D12).
- [ ] The types are immutable, with read-only collections (D14).

## Genericity (no Great Britain assumptions)

- [ ] `Kotsu.Scheduling` contains no `enum` declarations (search: `enum ` in `backend/Kotsu.Scheduling`).
- [ ] No constant, static field or `switch` in `Kotsu.Scheduling` names a country, stage or board (code review).
- [ ] Great Britain names appear only in the test project's fixture.

## Tests and build

- [ ] Every test follows Arrange, Act, Assert with comment markers, and asserts on error `Code`s, not on `Message`s (code review).
- [ ] `ScaffoldCheck` and its test are gone.
- [ ] `Kotsu.Scheduling` has no `PackageReference` and no `ProjectReference`.
- [ ] `dotnet build` gives 0 warnings and 0 errors. `dotnet format --verify-no-changes` exits 0. `dotnet test` passes.
- [ ] CI's `build-test-lint` is green on the PR.

## Evidence

Fill this in before you mark the PR ready.

| Item | Link or output |
| --- | --- |
| PR | |
| Green CI run | |
| Local `dotnet test` summary line | `Passed!  - Failed: 0, Passed: 94, Skipped: 0, Total: 94 - Kotsu.Scheduling.Tests.dll (net11.0)` |
| `enum` search in `backend/Kotsu.Scheduling` (expected: no matches) | No matches. Great Britain names appear only in two doc comments as examples (`ExamBoard.cs`, `QualificationStage.cs`). |
