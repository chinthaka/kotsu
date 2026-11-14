# Phase 4 — Plan

Branch: `features/2026-11-12-4-even-spread-scheduling`, from `main` after Phase 3 was merged. See [requirement.md](requirement.md) for the decisions (D1–D15) and [validation.md](validation.md) for the merge criteria and the test coverage.

Every section is test-first. Write the failing tests for its rules, run them and see them fail, then write the least code that passes, then tidy up. Tests use Arrange, Act, Assert with comment markers. Commit at the end of each section.

How the code is shaped, and what things are called, is up to the developers. This plan says what each step must do and how it is checked.

Unless a test says otherwise, the examples use phase 3's calendar: a 1-week window from Monday 2 November 2026, with Monday to Thursday at 2 hours, Friday at 1.5 hours, Saturday at 4 hours and Sunday at 0 hours. That is 13.5 hours, or 27 half-hours. All subjects share one made-up stage; different subjects may use different made-up boards. Subjects are given in the order listed.

A small helper in the test project builds a syllabus from a subject and a list of sub-topic hours, so tests stay short.

## 1. Subject selection

1.1 Tests (D2, D4): selecting the whole syllabus selects every sub-topic, in syllabus order, across all its topics. The selection knows which subject it is for.

1.2 Tests (D2, D4): selecting specific sub-topics selects only those, in syllabus order, even when they are given in reverse.

1.3 Test (D3): a sub-topic given twice is selected once.

1.4 Tests (D3): an empty list is rejected, and a sub-topic that isn't in the syllabus is rejected. Each has its own stable code, and the tests check the code only, never the wording.

1.5 Implement the subject selection.

## 2. Session and schedule shape

2.1 Test (D9): two sessions with the same date, subject, sub-topic and hours are equal.

2.2 Implement the session and the schedule's shape (D10): the window, the sessions, the hours on a date, the hours for a subject, and the unused hours. Only the scheduler can make a schedule, so the schedule is tested through section 3 onwards.

## 3. Schedule rules

3.1 Tests (D5): the scheduler rejects an empty list of selections, a subject given twice, and selections whose syllabi have different stages. Each has its own stable code.

3.2 Test (D5): two selections for different subjects with the same stage but different boards are accepted.

3.3 Test (D5): a duplicate subject and a mixed stage in one call report both problems.

3.4 Tests (D11): 3 selected sub-topics in a window with 1 hour in total (2 half-hours) are rejected with a "not enough time" code. An all-zero week is rejected with the same code and doesn't crash.

3.5 Implement the checks.

## 4. Sharing hours across subjects

Each test checks the hours given to every subject and the unused hours.

4.1 Test (D6, uneven split): Maths needs 10 hours, English needs 10 hours. Maths gets 7 hours, English 6.5, nothing unused. With English given first, English gets 7 and Maths 6.5.

4.2 Test (D6, need met): Maths has one 2-hour sub-topic, English one 20-hour sub-topic. Maths gets 2 hours, English 11.5, nothing unused.

4.3 Test (D6, spare goes to the others evenly): A needs 1 hour, B and C need 20 hours each. A gets 1 hour, B 6.5 hours, C 6 hours.

4.4 Test (D6, everyone's need met): Maths needs 3 hours, English 4 hours. Each gets exactly what it needs, and 6.5 hours are unused.

4.5 Test (D6, lower limit): a week of 1 hour Monday to Saturday and 0 on Sunday (6 hours, 12 half-hours). A has 10 sub-topics of 0.5 hours; B has one 20-hour sub-topic. A gets 5 hours and B gets 1, and every sub-topic is scheduled.

4.6 Implement the sharing step.

## 5. Splitting a subject's hours across its sub-topics

5.1 Test (D7): in the worked example, M1 and M2 get 3.5 hours each; E1 gets 2.5, E2 and E3 get 2 each (the spare half-hour goes to the first sub-topic).

5.2 Test (D7): a subject given more time than one sub-topic needs still splits evenly. One 0.5-hour and one 3.5-hour sub-topic, given 4 hours, get 2 hours each. This test is expected to change in phase 5.

5.3 Implement the splitting step.

## 6. Placing on dates

6.1 Test (D8, D9, D10): the worked example in [requirement.md](requirement.md) gives exactly the sessions in its table, in the schedule's session order, and nothing on Sunday.

6.2 Test (D8): with the "everyone's need met" inputs from 4.4, all work is placed by Thursday 5 November, Thursday has 1 hour of work and 1 unused, and Friday and Saturday have no sessions.

6.3 Test (D8): a window starting on Wednesday 4 November 2026 places its first sessions on that Wednesday, not on a Monday.

6.4 Tests (D10): the hours on a date equal the sum of that date's sessions, and are zero for a date with none or outside the window. The hours for a subject not in the schedule are zero.

6.5 Implement the placing step.

## 7. The "Done when" checks

7.1 Write one set of invariant checks that takes a schedule and the inputs it was built from, and checks all of these:
- Every selected sub-topic has at least one session, and no other sub-topic has any (exactly once, part 1).
- A sub-topic's sessions are on consecutive study dates, with no other sub-topic of the same subject between its first and last session (exactly once, part 2; D9).
- Every session is inside the window, on a date with more than zero hours, and has more than zero hours.
- No date's sessions add up to more than that date's hours.
- Each subject's sessions add up to the hours the schedule says that subject was given, and the window's hours equal the hours placed plus the unused hours.

7.2 Run the invariant checks as one data-driven test over at least these inputs: the worked example; the inputs from 4.2 to 4.5; a 4-week window with 3 subjects; a window starting mid-week; a week with every day at 0.5 hours; a single subject with one sub-topic.

7.3 Test (D12): building the same inputs twice gives the same sessions.

7.4 Fix anything the tests turn up, then tidy up. Keep the sharing, splitting and placing steps separate in the code (requirement.md, Risks).

## 8. Review and PR

8.1 Check the scheduling library:
- Days of the week are never listed Sunday-first (phase 3).
- Nothing reads the current date or time (D13).
- No enumerations are declared.
- No unordered collection is iterated to produce output order (D12).
- No package or project references.

8.2 Check the tests catch real bugs. Make each change below, see at least one test fail, then revert:
- Give the spare half-hour to the last subject instead of the first.
- Drop the "need" limit, so a subject can be given more than it needs.
- Always start each date with the first subject.
- Let a date take one half-hour more than its hours.

8.3 Make sure the build has 0 warnings, the formatting check passes, and all tests pass locally.

8.4 Push and open a PR to `main`, then confirm CI is green.

8.5 Fill in the evidence section of [validation.md](validation.md).
