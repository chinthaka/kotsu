# Phase 4 — Plan

Branch: `feature/phase-4`, from `main` after Phase 3 was merged. See [requirement.md](requirement.md) for the decisions (D1–D15) and [validation.md](validation.md) for the merge criteria.

Every section is test-first. Write the failing tests for its rules, run them and see them fail, then write the least code that passes, then tidy up. Tests use Arrange, Act, Assert with comment markers. Commit at the end of each section.

Unless a test says otherwise, the examples use phase 3's calendar: a 1-week window from Monday 2 November 2026, with Monday to Thursday at 2 hours, Friday at 1.5 hours, Saturday at 4 hours and Sunday at 0 hours. That is 13.5 hours, or 27 slots. All subjects share one made-up stage; different subjects may use different made-up boards. Subjects are given in the order listed.

A small test helper builds a syllabus from a subject id and a list of sub-topic hours, so tests stay short. It lives in the test project only.

## 1. SubjectSelection

1.1 Tests (D2, D4): `Create(syllabus)` selects every sub-topic, in syllabus order, across all its topics. `SubjectId` is the syllabus's subject.
1.2 Tests (D2, D4): `Create(syllabus, ids)` selects only those sub-topics, in syllabus order, even when the ids are given in reverse.
1.3 Test (D3): a repeated id is merged, so the sub-topic is selected once.
1.4 Tests (D3): an empty list fails with `subject-selection.no-sub-topics`. An id not in the syllabus fails with `subject-selection.unknown-sub-topic`. The tests check `Code` only.
1.5 Implement `SubjectSelection`.

## 2. StudySession and Schedule shape

2.1 Test (D9): two `StudySession`s with the same date, subject, sub-topic and hours are equal.
2.2 Implement `StudySession` as a `readonly record struct`, and `Schedule` with a private constructor, `Window`, `Sessions`, `HoursOn`, `HoursFor` and `UnusedHours` (D10). `Schedule` is only made by the scheduler, so it has no public `Create`; it is tested through section 3 onwards.

## 3. Schedule rules

3.1 Tests (D5): `Build` fails with `schedule.no-subjects` for an empty list, `schedule.duplicate-subject` when a subject is given twice, and `schedule.mixed-stage` when two syllabi have different stages.
3.2 Test (D5): two selections for different subjects with the same stage but different boards succeed.
3.3 Test (D5): a duplicate subject and a mixed stage in one call report both codes.
3.4 Tests (D11): 3 selected sub-topics in a window with 1 hour in total (2 slots) fail with `schedule.not-enough-hours`. An all-zero week fails with the same code and doesn't throw.
3.5 Implement the checks in `EvenSpreadScheduler.Build`.

## 4. Sharing hours across subjects

Each test checks `HoursFor` for every subject and `UnusedHours`.

4.1 Test (D6, uneven split): Maths needs 10 hours, English needs 10 hours. Maths gets 7 hours, English 6.5, nothing unused. With English given first, English gets 7 and Maths 6.5.
4.2 Test (D6, need met): Maths has one 2-hour sub-topic, English one 20-hour sub-topic. Maths gets 2 hours, English 11.5, nothing unused.
4.3 Test (D6, spare goes to the others evenly): A needs 1 hour, B and C need 20 hours each. A gets 1 hour, B 6.5 hours, C 6 hours.
4.4 Test (D6, everyone's need met): Maths needs 3 hours, English 4 hours. Each gets exactly what it needs, and `UnusedHours` is 6.5.
4.5 Test (D6, lower limit): a week of 1 hour Monday to Saturday and 0 on Sunday (6 hours, 12 slots). A has 10 sub-topics of 0.5 hours; B has one 20-hour sub-topic. A gets 5 hours and B gets 1, and every sub-topic is scheduled.
4.6 Implement the share step.

## 5. Splitting a subject's hours across its sub-topics

5.1 Test (D7): in the worked example, M1 and M2 get 3.5 hours each; E1 gets 2.5, E2 and E3 get 2 each (the spare slot goes to the first sub-topic).
5.2 Test (D7): a subject given more time than one sub-topic needs still splits evenly. One 0.5-hour and one 3.5-hour sub-topic, given 4 hours, get 2 hours each. This test is expected to change in phase 5.
5.3 Implement the split step.

## 6. Placing on dates

6.1 Test (D8, D9, D10): the worked example in [requirement.md](requirement.md) gives exactly the sessions in its table, in `Sessions` order, and nothing on Sunday.
6.2 Test (D8): with the "everyone's need met" inputs from 4.4, all work is placed by Thursday 5 November, Thursday has 1 hour of work and 1 unused, and Friday and Saturday have no sessions.
6.3 Test (D8): a window starting on Wednesday 4 November 2026 places its first sessions on that Wednesday, not on a Monday.
6.4 Test (D10): `HoursOn` returns the sum of a date's sessions, and zero for a date with none or outside the window. `HoursFor` returns zero for a subject not in the schedule.
6.5 Implement the placing step.

## 7. The "Done when" checks

7.1 Write one invariant helper that checks a schedule against its inputs:
- Every selected sub-topic has at least one session, and no other sub-topic has any (exactly once, part 1).
- A sub-topic's sessions are on consecutive study dates, with no other sub-topic of the same subject between its first and last session (exactly once, part 2; D9).
- Every session is inside the window, on a date with more than zero hours, and has more than zero hours.
- No date's sessions add up to more than that date's hours.
- Each subject's sessions add up to `HoursFor` for that subject, and the window's hours equal the hours placed plus `UnusedHours`.

7.2 Run the helper as an xUnit theory over at least these inputs: the worked example; 4.2 to 4.5; a 4-week window with 3 subjects; a window starting mid-week; a week with every day at 0.5 hours; a single subject with one sub-topic.
7.3 Test (D12): building the same inputs twice gives equal `Sessions`.
7.4 Fix anything the tests turn up, then tidy up. Keep the share, split and place steps as separate private methods (requirement.md, Risks).

## 8. Review and PR

8.1 Check `Kotsu.Scheduling`:
- No `Enum.GetValues<DayOfWeek>()`.
- No `DateTime.Now`, `DateTime.Today`, `DateTime.UtcNow` or `TimeProvider` (D13).
- No `enum` declarations.
- No `HashSet` or `Dictionary` iterated to produce output order (D12).
- No package or project references.

8.2 Check the tests catch real bugs. Make each change below, see at least one test fail, then revert:
- Give the spare slot to the last subject instead of the first.
- Drop the "need" limit, so a subject can be given more than it needs.
- Always start each date with the first subject.
- Let a date take one slot more than its hours.

8.3 Make sure the build has 0 warnings, `dotnet format --verify-no-changes` passes, and `dotnet test` is green locally.
8.4 Push and open a PR to `main`, then confirm CI's `build-test-lint` is green.
8.5 Fill in the evidence section of [validation.md](validation.md).
