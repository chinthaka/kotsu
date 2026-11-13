# Phase 4 — Requirements

Roadmap phase 4 ([roadmap.md](../roadmap.md)), Stage 1: Scheduling engine.

## Goal

Even-spread scheduling: place every selected topic into the available hours, splitting the hours evenly across subjects.

**Done when** (from the roadmap): every selected topic is scheduled exactly once, inside the window; no day is given more work than its hours; each subject gets an even share, and hours a subject does not need go to the others.

## Context

- Phase 3 is merged. `StudyCalendar` gives the hours on every date of the window (`Days`, `HoursOn`, `TotalHours`), with zero-hour days included.
- Phase 2 gives the content: a `Syllabus` (one stage, subject and board) holds `Topic`s, and each `Topic` holds `SubTopic`s with their `EstimatedHours` (at least 0.5). `StudyHours` is a `decimal` in half-hour steps. Every type is made through `Create`, which returns `Result<T>` and reports every broken rule.
- Phase 2 left the student's topic selection to this phase: "the whole syllabus or specific sub-topics" ([mission.md](../mission.md), Topic selection).
- A sub-topic is "the smallest unit of a syllabus a student selects and the engine schedules" (phase 2). Where the roadmap says "topic" in this phase, it means a selected sub-topic.
- [mission.md](../mission.md): one schedule covers many subjects, "within one country, stage and board", sharing the weekly hours. "The hours are split evenly at first; a priority rule may replace this" (phase 7).
- Phase 5 makes each topic's time proportional to its estimated hours. Phase 4 doesn't: inside a subject, time is split evenly across its sub-topics (D7). Phase 4's output is never shown to a student, so this is safe.
- Phase 6 defines the edge cases (single-day window, more topics than days, no topics, no hours). Phase 4 only has to return a defined failure for them, never throw (D11).
- Phases 11–12 add recap. Hours left over after every subject has what it needs are reported as unused (D10), which is where recap can go later.

## Scope

In scope:

1. `SubjectSelection`: one subject's syllabus and the sub-topics the student selected, either the whole syllabus or a list of sub-topic ids.
2. `StudySession`: a date, a subject, a sub-topic and the hours given to it that day.
3. `Schedule`: the window, the sessions, and the hours each subject was given and left unused.
4. `EvenSpreadScheduler.Build(StudyCalendar, IReadOnlyList<SubjectSelection>)`, which returns `Result<Schedule>`.
5. Tests that turn the roadmap's "Done when" into checks, plus a worked example with every session listed.

Out of scope:

- Weighting by estimated hours (phase 5).
- The edge cases' final behaviour (phase 6). Phase 4 returns a failure for them (D11), and phase 6 may change that.
- A priority rule across subjects (phase 7).
- Progress, "today", rescheduling and missed days (phases 8–10). `Build` takes no `Progress` and no date other than the window.
- Recap sessions and the recap flag (phases 11–12).
- Choosing an order of study across topics other than the syllabus order (D8).
- Anything about how the schedule is shown to the student.

## Decisions

Decisions marked **Proposed** need confirming in the phase 4 review.

| # | Decision | Notes |
| --- | --- | --- |
| D1 | **Time is shared in half-hour slots.** The engine turns hours into half-hour slots, shares and places whole slots, and turns them back into `StudyHours`. | `StudyHours` and every day's hours are already in half-hour steps (phase 2 D3, phase 3 D12), so no rounding is needed below a slot and every result is a valid `StudyHours`. |
| D2 | **`SubjectSelection`.** `SubjectSelection.Create(Syllabus syllabus)` selects the whole syllabus. `SubjectSelection.Create(Syllabus syllabus, IEnumerable<SubTopicId> subTopicIds)` selects specific sub-topics. Both return `Result<SubjectSelection>`. It exposes `Syllabus`, `SubjectId` and `SubTopics` (the selected `SubTopic`s). | This follows phase 2 D1. The selection holds the syllabus so the engine needs no content lookup and stays pure. |
| D3 | Selection rules: the specific list is not empty (`subject-selection.no-sub-topics`), and every id belongs to the syllabus (`subject-selection.unknown-sub-topic`). Duplicate ids are merged, not rejected. | Merging follows `Progress` (phase 2 D13): selecting a sub-topic twice is harmless. |
| D4 | **Selected sub-topics are kept in syllabus order,** whatever order the ids were given in. | The curated order is the order a subject is usually taught, and it makes the result deterministic. |
| D5 | **Schedule rules.** `Build` fails when: there are no selections (`schedule.no-subjects`); the same subject is selected twice (`schedule.duplicate-subject`); the selections don't share one stage (`schedule.mixed-stage`) or one board (`schedule.mixed-board`). | [mission.md](../mission.md): "Many, within one country, stage and board." Every broken rule is reported, as in phase 2 D2. |
| D6 | **Each subject's share** (**Proposed**). Each subject gets an even share of the window's slots, with two limits: never fewer slots than it has selected sub-topics (one half-hour each), and never more than it needs (the sum of its selected sub-topics' estimated hours). Slots a subject doesn't need go to the subjects that still need more, evenly. When an even share doesn't divide into whole slots, the spare slots go one each to subjects in the order they were given. | This is the roadmap's "even share, and hours a subject does not need go to the others". The lower limit makes sure every selected sub-topic can be scheduled exactly once whenever the window has at least one slot per sub-topic. Without it, a subject with many small sub-topics could be squeezed out while another subject's share went unused. The alternative is a pure even split that fails in that case, leaving it to phase 6. |
| D7 | **Inside a subject, time is split evenly across its selected sub-topics** (**Proposed**). Each gets the subject's slots divided by the number of sub-topics; spare slots go one each to the earliest sub-topics in syllabus order. Every sub-topic gets at least one slot (D6). | Phase 4 is "even spread". Phase 5 replaces this split with one proportional to estimated hours, which is its "Done when". A small sub-topic can get more time than it needs here; that's expected until phase 5. |
| D8 | **Placing on dates** (**Proposed**). Dates are filled in date order. Each date's slots are handed out one at a time, in turn, to the subjects that still have slots to place. The subject that goes first moves along by one each date (date 1 starts with the first subject, date 2 with the second, and so on, skipping subjects that have finished). Inside a subject, sub-topics are placed in syllabus order, and a sub-topic carries on into the next date if it doesn't fit. | Every date is shared across subjects, as the mission asks, and no subject is always first. Work is placed as early as possible, so spare hours fall at the end of the window, where recap can go later (phase 11). |
| D9 | **Sessions.** A `StudySession` is a `readonly record struct StudySession(DateOnly Date, SubjectId SubjectId, SubTopicId SubTopicId, StudyHours Hours)`. One sub-topic on one date is one session, however many slots it has that date. Every session has more than zero hours. A sub-topic placed across several dates has one session per date, on consecutive study dates, with no other sub-topic of the same subject in between. | "Scheduled exactly once" means each selected sub-topic is placed in one continuous run, not repeated or revisited. A study date is a date with more than zero hours. |
| D10 | **`Schedule`** exposes `Window`, `Sessions` (date order, then subject in the order given, then syllabus order), `HoursOn(DateOnly date)`, `HoursFor(SubjectId subjectId)` and `UnusedHours`. `HoursOn` and `HoursFor` return `StudyHours.Zero` for a date or subject not in the schedule. | `UnusedHours` is the window's hours minus the hours placed. It is zero when time is short, and is where recap goes in phase 11. Phase 3 D5 set the same "zero, not an exception" rule for dates. |
| D11 | **Not enough time.** When the window has fewer slots than there are selected sub-topics, `Build` fails with `schedule.not-enough-hours`. This includes a window with no hours at all. | A failed `Result` is a defined result, not an exception. Phase 6 decides each edge case's final behaviour. |
| D12 | **Deterministic.** The same inputs always give the same schedule. Nothing is random, and nothing depends on hash-set or dictionary order. | Phase 21 checks the API returns the same schedule as the engine, and tests compare whole schedules. |
| D13 | The engine never reads the clock. No `DateTime.Now`, `DateTime.Today`, `DateTime.UtcNow` or `TimeProvider` in `Kotsu.Scheduling`. | Carried from phase 3 D9. |
| D14 | `SubjectSelection`, `StudySession` and `Schedule` live in `backend/Kotsu.Scheduling/Domain/` (namespace `Kotsu.Scheduling.Domain`). `EvenSpreadScheduler` is a static class in `backend/Kotsu.Scheduling/Engine/` (namespace `Kotsu.Scheduling.Engine`). Tests mirror this in `backend/tests/Kotsu.Scheduling.Tests/Domain/` and `backend/tests/Kotsu.Scheduling.Tests/Engine/`. | Domain types follow phase 2 D15. The scheduler is behaviour, not a domain type, and phases 5, 7 and 8 change it, so it gets its own folder. |
| D15 | The types are immutable, and every collection is read-only. | Carried from phase 2 D14. |

## Worked example

This is the example the plan's main test uses. A 1-week window from Monday 2 November 2026, with phase 3's week: Monday to Thursday 2 hours, Friday 1.5, Saturday 4, Sunday 0. That is 13.5 hours, or 27 slots.

- Maths (given first): sub-topics M1 and M2, 5 hours each. It needs 10 hours.
- English: sub-topics E1 (4 hours), E2 (3 hours) and E3 (3 hours). It needs 10 hours.

Shares (D6): 27 slots don't split evenly across 2 subjects, so Maths gets 14 slots (7 hours) and English 13 (6.5 hours). Neither gets all it needs. Inside subjects (D7): M1 and M2 get 3.5 hours each; E1 gets 2.5 hours, and E2 and E3 get 2 hours each.

Placing (D8):

| Date | Starts with | Maths | English |
| --- | --- | --- | --- |
| Mon 2 Nov (2h) | Maths | M1 1h | E1 1h |
| Tue 3 Nov (2h) | English | M1 1h | E1 1h |
| Wed 4 Nov (2h) | Maths | M1 1h | E1 0.5h, E2 0.5h |
| Thu 5 Nov (2h) | English | M1 0.5h, M2 0.5h | E2 1h |
| Fri 6 Nov (1.5h) | Maths | M2 1h | E2 0.5h |
| Sat 7 Nov (4h) | English | M2 2h | E3 2h |
| Sun 8 Nov (0h) | — | — | — |

`UnusedHours` is zero.

## Constraints

- `Kotsu.Scheduling` keeps zero package references and zero project references.
- No `enum` declarations, and no `Enum.GetValues<DayOfWeek>()` (phase 3 D13).
- Every test follows Arrange, Act, Assert, with comment markers (tech-stack.md).
- The build has zero warnings, lint passes, and CI is green.
- Nothing names a country, stage, board or subject. Subjects, stages and boards in tests are made-up data.
- Nothing identifies a student. The inputs are content, dates and hours only.

## Risks

- **Building phase 5 early.** It is tempting to weight sub-topics by size now. Don't: D7 splits evenly, and phase 5's tests are what change it. Keep the share step (D6), the split step (D7) and the placing step (D8) as separate private methods so phases 5 and 7 can each replace one.
- **Rounding drift.** Splitting in decimals and rounding at the end can lose or gain half an hour. Working in whole slots (D1) avoids this; tests check that every subject's sessions add up to its share and every day's sessions to no more than its hours.
- **"Even share" tested only where it's easy.** Tests cover an uneven split (spare slot), a subject that needs less than its share, and the lower limit in D6, not only two equal subjects.
- **Hidden order dependence.** Grouping with dictionaries or sets can change the order of sessions between runs. D12 is checked by building the same schedule twice and by a test that gives sub-topic ids out of syllabus order.
- **The proposed decisions change in review.** D6, D7 and D8 are the parts most likely to change. The worked example and the tests that depend on it are rewritten if they do.

## Open questions for the review

1. **D6's lower limit.** Keep "at least one half-hour per selected sub-topic", or use a pure even split and fail when a subject can't fit its sub-topics?
2. **D8's placing.** Place work as early as possible (spare time at the end), or spread it across the whole window (spare time scattered)? Early placing leaves room for recap and for catching up after a missed day.
3. **Mixed boards in England.** The mission says one board per schedule. Real students in England often take subjects with different boards. Phase 4 follows the mission (D5); is that still the intent?
