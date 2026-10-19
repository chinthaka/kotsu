# Phase 3 — Validation

The PR for `feature/phase-3` can merge when every check below passes and the evidence section is filled in.

## Roadmap "Done when"

- [ ] Every date in the window has exactly the hours entered for its weekday (D2, D4), shown by tests that check each date in a 1-week window and the first and last dates of a 4-week window.
- [ ] A day with zero hours has none: it appears in `Days` with `StudyHours.Zero` (D3), and an all-zero week gives a valid calendar with zero `TotalHours`.

## Design

- [ ] `StudyCalendar` is created through `Create(window, availability)`, which returns `Result<StudyCalendar>` (D1).
- [ ] `Days` has 7 × `Weeks` entries, in date order, from `StartDate` to `EndDate` inclusive, across month and year boundaries (D2).
- [ ] `HoursOn` returns `StudyHours.Zero` for a date outside the window, and doesn't throw (D5).
- [ ] `TotalHours` equals the sum of `Days` (D6), and `Window` is the window that was passed in (D7).
- [ ] The types are immutable, and `Days` is read-only (D11).
- [ ] Half-hour days carry through unchanged: a 1.5-hour weekday gives 1.5 hours on its dates, and `TotalHours` is exact (D12).
- [ ] `IsoWeekdays.All` runs Monday to Sunday, `WeeklyAvailability.Create` lists missing days Monday first, and `Kotsu.Scheduling` has no `Enum.GetValues<DayOfWeek>()` (D13).
- [ ] A window starting mid-week is still accepted and runs in date order (D13).

## Tests and build

- [ ] Every test follows Arrange, Act, Assert with comment markers (code review).
- [ ] `Kotsu.Scheduling` has no `PackageReference` and no `ProjectReference`.
- [ ] `dotnet build` gives 0 warnings and 0 errors. `dotnet format --verify-no-changes` exits 0. `dotnet test` passes.
- [ ] CI's `build-test-lint` is green on the PR.

## Evidence

Fill this in before you mark the PR ready.

| Item | Link or output |
| --- | --- |
| PR | https://github.com/chinthaka/kotsu/pull/4 |
| Green CI run | https://github.com/chinthaka/kotsu/actions/runs/39295095148 (`build-test-lint` passed) |
| Local `dotnet test` summary line | `Passed!  - Failed: 0, Passed: 115, Skipped: 0, Total: 115 - Kotsu.Scheduling.Tests.dll (net11.0)` |
| Searches in `backend/Kotsu.Scheduling` (expected: no matches) | `GetValues<DayOfWeek>`, `DateTime.Now`, `DateTime.Today`, `DateTime.UtcNow`, `TimeProvider`, `enum` declarations, `PackageReference` and `ProjectReference`: no matches. |
| Boundary tests catch real bugs | With the last day dropped from the window, 8 tests failed. With each date given the hours for its position in the window instead of its weekday, the Wednesday-start test failed. Both changes were reverted. |
