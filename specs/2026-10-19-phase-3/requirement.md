# Phase 3 — Requirements

Roadmap phase 3 ([roadmap.md](../roadmap.md)), Stage 1: Scheduling engine.

## Goal

Study availability: turn the schedule window and the hours for each day of the week into the hours available on each date.

**Done when** (from the roadmap): every date in the window has exactly the hours entered for its weekday, and a day with zero hours has none.

## Context

- Phase 2 is merged. `Kotsu.Scheduling.Domain` has `ScheduleWindow` (a `StartDate` and 1–4 `Weeks`, with an inclusive `EndDate`), `WeeklyAvailability` (hours for all 7 `DayOfWeek`s, `HoursOn(day)`, `TotalHours`) and `StudyHours` (a `decimal` in half-hour steps, which can be added and compared).
- [tech-stack.md](../tech-stack.md): the engine is a pure function with no I/O. Schedule windows are calendar dates with no time zone. Work is test-first, and every test follows Arrange, Act, Assert.
- [mission.md](../mission.md): the student enters how many hours they can study on each of the 7 days, and the schedule is built from those hours.
- Phase 2 already allows half hours. `StudyHours` comes in half-hour steps (phase 2 D3), and a day takes 0 to 24 hours (phase 2 D4), so a student can enter 0.5, 1.5 or 2.5 hours for a day.
- .NET's `DayOfWeek` numbers Sunday as 0, so iterating `Enum.GetValues<DayOfWeek>()` starts on Sunday. The only place this shows today is the "missing day" message in `WeeklyAvailability.Create`.
- Phase 4 places selected topics into these per-date hours, making sure no day gets more work than its hours. Phase 6 needs the "no study hours at all" case (an all-zero week) to give a valid calendar.

## Scope

In scope:

1. `DailyHours`, a date and the study hours available on it.
2. `StudyCalendar`, built from a `ScheduleWindow` and a `WeeklyAvailability`, holding one `DailyHours` for every date in the window.
3. `IsoWeekdays`, the 7 days of the week in ISO-8601 order (Monday to Sunday). `WeeklyAvailability.Create` is changed to use it, so missing days are listed Monday first.
4. Tests that turn the roadmap's "Done when" into checks, including half-hour days, month and year boundaries, and an all-zero week.

Out of scope:

- Any placing of topics, and any split across subjects (phase 4 onwards).
- Days off on particular dates, such as holidays or one-off exceptions. The mission only has weekly hours.
- "Today" and rescheduling from part-way through the window (phase 8).
- Recap time (phases 11–12).
- Requiring a window to start on a Monday. A window still starts on any day (D13).
- Amounts smaller than half an hour. That would change `StudyHours` (phase 2 D3), which this phase doesn't do.

## Decisions

| # | Decision | Notes |
| --- | --- | --- |
| D1 | **A new `StudyCalendar` type** is created with `StudyCalendar.Create(ScheduleWindow window, WeeklyAvailability availability)`, which returns `Result<StudyCalendar>`. | This was chosen in the phase 3 review. It follows phase 2's D1 so that every domain type is made the same way. Both inputs are already valid, so `Create` always succeeds, as `Progress.Create` does. |
| D2 | `StudyCalendar.Days` is an `IReadOnlyList<DailyHours>` with exactly one entry per date from `StartDate` to `EndDate` inclusive (7 × `Weeks` entries), in date order. | |
| D3 | **Zero-hour days are included** with `StudyHours.Zero`, not left out. | This was chosen in the phase 3 review. "Every date in the window" stays literally true, and phase 4 can see rest days. |
| D4 | Each date's hours are `availability.HoursOn(date.DayOfWeek)`. Nothing else changes them. | This is the roadmap's "exactly the hours entered for its weekday". |
| D5 | `StudyCalendar.HoursOn(DateOnly date)` returns that date's hours. A date outside the window returns `StudyHours.Zero`. | A date outside the window has no study time in the schedule. Returning zero means phase 4 has no exception path to handle (phase 6: "never an exception"). |
| D6 | `StudyCalendar.TotalHours` is the sum of every day's hours. It is derived when the calendar is created. | Phase 4 needs the total to share hours across subjects. |
| D7 | `StudyCalendar` keeps its `Window`. | Later phases (8, 9) work with dates relative to the window, and this saves passing the window alongside the calendar. |
| D8 | `DailyHours` is a `readonly record struct DailyHours(DateOnly Date, StudyHours Hours)` with no `Create`. | It has no rules of its own: both parts are already valid types. |
| D9 | The engine never reads the clock. No `DateTime.Now`, `DateTime.Today` or `TimeProvider` in `Kotsu.Scheduling`. | The engine stays a pure function. "Today" is passed in when phase 8 needs it. |
| D10 | The types live in `backend/Kotsu.Scheduling/Domain/` (namespace `Kotsu.Scheduling.Domain`). Tests go in `backend/tests/Kotsu.Scheduling.Tests/Domain/StudyCalendarTests.cs`. | This follows phase 2's D15. |
| D11 | The types are immutable, and `Days` is a read-only list. | This follows phase 2's D14. |
| D12 | **Half hours carry through.** A date's hours are copied from its weekday as they are, so a weekday entered as 1.5 hours gives 1.5 hours on every matching date. `TotalHours` is exact, because `StudyHours` is a `decimal`. | This was clarified in the phase 3 review. The smallest amount a date can carry is half an hour, and phase 4 can place work in half-hour steps. |
| D13 | **Weeks start on Monday (ISO-8601).** Wherever the engine lists or iterates days of the week, the order is Monday to Sunday, taken from `IsoWeekdays.All`. It never iterates `Enum.GetValues<DayOfWeek>()`, which starts on Sunday. A schedule window still starts on any day. | This was chosen in the phase 3 review: ordering only, not a Monday start rule. `Days` is in date order, so a window starting on a Wednesday runs Wednesday to Tuesday. |

## Constraints

- `Kotsu.Scheduling` keeps zero package references and zero project references.
- Every test follows Arrange, Act, Assert, with comment markers (tech-stack.md).
- The build has zero warnings, lint passes, and CI is green.
- Nothing assumes Great Britain. A window can start on any weekday. Where days of the week are ordered, the week starts on Monday (D13).

## Risks

- **Off-by-one at the window edges.** `EndDate` is inclusive, so a loop that stops before `EndDate`, or runs one day past it, is easy to write. Tests check the first and last entries and the count for each window length.
- **Building ahead of phase 4.** Keep `StudyCalendar` to `Window`, `Days`, `HoursOn` and `TotalHours`, and `IsoWeekdays` to the ordered list. ISO week numbers aren't needed yet: .NET's `System.Globalization.ISOWeek` can supply them when a later phase does.
- **Sunday-first order creeping back.** A new `Enum.GetValues<DayOfWeek>()` loop would quietly bring it back. The review step searches for it.
