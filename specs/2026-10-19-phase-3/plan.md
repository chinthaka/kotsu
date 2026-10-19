# Phase 3 — Plan

Branch: `feature/phase-3`, from `main` after Phase 2 was merged. See [requirement.md](requirement.md) for the decisions (D1–D13) and [validation.md](validation.md) for the merge criteria.

Every group is test-first. Write the failing tests for its rules, run them and see them fail, then write the least code that passes, then tidy up. Tests use Arrange, Act, Assert with comment markers. Commit at the end of each group.

Unless a test says otherwise, the examples use a 1-week window from Monday 2 November 2026, and this week: Monday to Thursday at 2 hours, Friday at 1.5 hours, Saturday at 4 hours and Sunday at 0 hours. The weekly total is 13.5 hours.

## 1. ISO weekday order

1.1 Tests (D13): `IsoWeekdays.All` has 7 days, starts with Monday, ends with Sunday, and has no repeats.
1.2 Test: `WeeklyAvailability.Create` with both Sunday and Monday missing reports them Monday first. The test checks the error `Code` and the order of the days listed. The message wording isn't checked.
1.3 Implement `IsoWeekdays`, and change `WeeklyAvailability.Create` to use it in place of `Enum.GetValues<DayOfWeek>()`.

## 2. DailyHours

2.1 Test: two `DailyHours` with the same date and hours are equal (D8).
2.2 Implement `DailyHours` as a `readonly record struct`.

## 3. StudyCalendar core

3.1 Tests: `Create` succeeds (D1). `Days` has 7 entries, in date order, from 2 November to 8 November 2026 (D2). Each date's hours are the hours for its weekday (D3, D4):
- Monday 2 November has 2 hours.
- Friday 6 November has 1.5 hours (D12).
- Saturday 7 November has 4 hours.
- Sunday 8 November has 0 hours, and is still in `Days`.

`Window` is the window that was passed in (D7).
3.2 Tests: `HoursOn(date)` returns the hours for a date inside the window. It returns `StudyHours.Zero` for the day before `StartDate` and for the day after `EndDate` (D5).
3.3 Test: `TotalHours` is exactly 13.5 hours for the example week (D6, D12).
3.4 Implement `StudyCalendar`.

## 4. Boundary cases

4.1 Test: a 4-week window has 28 entries, the first is `StartDate`, the last is `EndDate`, and `TotalHours` is exactly 54 hours (4 × 13.5).
4.2 Test: a window starting on Wednesday 4 November 2026 runs from that Wednesday to Tuesday 10 November, in date order, and maps each date to its own weekday, not to its position in the window (D13).
4.3 Tests: a window that crosses a month end (from Monday 23 November 2026, 2 weeks) and a window that crosses a year end (from Monday 28 December 2026, 1 week) have consecutive dates with no gaps or repeats.
4.4 Test: a week with every day at 0.5 hours gives 0.5 hours on every date and 3.5 hours in total over 1 week (D12).
4.5 Test: an all-zero week gives a valid calendar where every date has `StudyHours.Zero` and `TotalHours` is zero. This is the case phase 6 needs.
4.6 Fix anything the tests turn up, then tidy up.

## 5. Review and PR

5.1 Check `Kotsu.Scheduling`:
- No `Enum.GetValues<DayOfWeek>()` (D13).
- No `DateTime.Now`, `DateTime.Today` or `TimeProvider` (D9).
- No `enum` declarations.
- No package or project references.

5.2 Make sure the build has 0 warnings, `dotnet format --verify-no-changes` passes, and `dotnet test` is green locally.
5.3 Push and open a PR to `main`, then confirm CI's `build-test-lint` is green.
5.4 Fill in the evidence section of [validation.md](validation.md).
