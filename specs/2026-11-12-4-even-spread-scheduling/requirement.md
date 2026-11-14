# Phase 4 — Requirements

Roadmap phase 4 ([roadmap.md](../roadmap.md)), Stage 1: Scheduling engine.

This document describes what the engine must do, not how to code it. Names of types, methods, files and error codes are left to the developers, who follow the conventions set in phases 2 and 3.

## Goal

Even-spread scheduling: place every selected topic into the available hours, splitting the hours evenly across subjects.

**Done when** (from the roadmap): every selected topic is scheduled exactly once, inside the window; no day is given more work than its hours; each subject gets an even share, and hours a subject does not need go to the others.

## Context

- Phase 3 is merged. The engine can already turn a schedule window and the weekly hours into a study calendar: the hours available on every date of the window, with zero-hour days included, and the total hours for the window.
- Phase 2 gives the content. A syllabus belongs to one stage, one subject and one exam board, and holds topics. Each topic holds sub-topics, and each sub-topic has an estimated study time of at least half an hour. Study time is always measured in half-hour steps. Every domain object is created through a factory that either returns the object or a list of every broken rule, each with a stable code.
- Phase 2 left the student's topic selection to this phase: "the whole syllabus or specific sub-topics" ([mission.md](../mission.md), Topic selection).
- A sub-topic is "the smallest unit of a syllabus a student selects and the engine schedules" (phase 2). Where the roadmap says "topic" in this phase, it means a selected sub-topic.
- [mission.md](../mission.md): one schedule covers many subjects within one country and stage, sharing the weekly hours. Each subject has its own board, so boards may differ within a schedule. "The hours are split evenly at first; a priority rule may replace this" (phase 7).
- Phase 5 makes each topic's time proportional to its estimated hours. Phase 4 doesn't: inside a subject, time is split evenly across its sub-topics (D7). Phase 4's output is never shown to a student, so this is safe.
- Phase 6 defines the edge cases (single-day window, more topics than days, no topics, no hours). Phase 4 only has to return a defined failure for them, never crash (D11).
- Phases 11–12 add recap. Hours left over after every subject has what it needs are reported as unused (D10), which is where recap can go later.

## Scope

In scope:

1. **A subject selection**: one subject's syllabus together with the sub-topics the student selected, either the whole syllabus or a chosen list of sub-topics.
2. **A study session**: a date, a subject, a sub-topic and the hours given to that sub-topic on that date.
3. **A schedule**: the window, the list of sessions, the hours each subject was given, and the hours left unused.
4. **The even-spread scheduler**: takes a study calendar and one or more subject selections, and returns either a schedule or a list of problems.
5. Tests that turn the roadmap's "Done when" into checks, plus a worked example with every session listed.

Out of scope:

- Weighting by estimated hours (phase 5).
- The edge cases' final behaviour (phase 6). Phase 4 returns a failure for them (D11), and phase 6 may change that.
- A priority rule across subjects (phase 7).
- Progress, "today", rescheduling and missed days (phases 8–10). The scheduler takes no progress and no date other than the window.
- Recap sessions and the recap flag (phases 11–12).
- Choosing an order of study across topics other than the syllabus order (D8).
- Anything about how the schedule is shown to the student.

## Decisions

Decisions marked **Proposed** need confirming in the phase 4 review.

| # | Decision | Notes |
| --- | --- | --- |
| D1 | **Time is shared in half-hour slots.** The engine works in whole half-hours when sharing and placing time, and reports results in hours. | Study time and every day's hours are already in half-hour steps (phase 2, phase 3), so no rounding is ever needed below half an hour, and every result is a valid amount of study time. |
| D2 | **A subject selection** holds the subject's syllabus and the selected sub-topics. It can be made in two ways: select the whole syllabus, or select specific sub-topics by their ids. It is created the same way as every phase 2 type: a factory that returns either the selection or the broken rules. | The selection carries the syllabus with it so the engine needs no lookup and stays a pure function. |
| D3 | **Selection rules.** A specific list must not be empty, and every id in it must belong to the syllabus. Each broken rule has a stable code. Repeated ids are merged, not rejected. | Merging follows progress in phase 2: selecting a sub-topic twice is harmless. |
| D4 | **Selected sub-topics are kept in syllabus order,** whatever order the ids were given in. | The curated order is the order a subject is usually taught, and it makes the result the same every time. |
| D5 | **Schedule rules.** The scheduler fails when: there are no subject selections; the same subject is selected twice; or the selections don't all share one stage. Different subjects may use different boards, since each syllabus already names the board for its subject. Every broken rule is reported, each with a stable code. | [mission.md](../mission.md): one schedule covers many subjects within one country and stage, with each subject using its own board. Reporting every broken rule follows phase 2. |
| D6 | **Each subject's share** (**Proposed**). Each subject gets an even share of the window's half-hours, with two limits: never fewer half-hours than it has selected sub-topics (one half-hour each), and never more than it needs (the sum of its selected sub-topics' estimated hours). Half-hours a subject doesn't need go to the subjects that still need more, shared evenly among them. When an even share doesn't divide into whole half-hours, the spare half-hours go one each to subjects in the order they were given. | This is the roadmap's "even share, and hours a subject does not need go to the others". The lower limit makes sure every selected sub-topic can be scheduled whenever the window has at least one half-hour per sub-topic. Without it, a subject with many small sub-topics could be squeezed out while another subject's share went unused. The alternative is a pure even split that fails in that case, leaving it to phase 6. |
| D7 | **Inside a subject, time is split evenly across its selected sub-topics** (**Proposed**). Each gets the subject's half-hours divided by the number of sub-topics; spare half-hours go one each to the earliest sub-topics in syllabus order. Every sub-topic gets at least one half-hour (D6). | Phase 4 is "even spread". Phase 5 replaces this split with one proportional to estimated hours, which is its "Done when". A small sub-topic can get more time than it needs here; that's expected until phase 5. |
| D8 | **Placing on dates** (**Proposed**). Dates are filled in date order. Each date's half-hours are handed out one at a time, in turn, to the subjects that still have time to place. The subject that goes first moves along by one each date: the first date starts with the first subject, the second date with the second, and so on, skipping subjects that have finished. Inside a subject, sub-topics are placed in syllabus order, and a sub-topic carries on into the next date if it doesn't fit. | Every date is shared across subjects, as the mission asks, and no subject is always first. Work is placed as early as possible, so spare hours fall at the end of the window, where recap can go later (phase 11). |
| D9 | **Sessions.** A session is one sub-topic on one date, with the hours it gets that day, however many half-hours that is. Every session has more than zero hours. A sub-topic placed across several dates has one session per date, and its sessions form one unbroken run within its subject: no other sub-topic of the same subject comes between its first and last session. | "Scheduled exactly once" means each selected sub-topic is placed in one continuous run, not repeated or revisited. When a date has fewer half-hours than there are subjects, the turn-taking in D8 means other subjects may fall on the dates between, so the run is unbroken within the subject, not on the calendar. |
| D10 | **The schedule** gives: the window; the sessions, in date order, then by subject in the order given, then in syllabus order; the hours on any date; the hours given to any subject; and the unused hours. Asking for the hours on a date or subject that isn't in the schedule gives zero, not an error. | Unused hours are the window's hours minus the hours placed. They are zero when time is short, and are where recap goes in phase 11. Phase 3 set the same "zero, not an error" rule for dates outside the window. |
| D11 | **Not enough time.** When the window has fewer half-hours than there are selected sub-topics, the scheduler fails with a stable code. This includes a window with no hours at all. | A reported failure is a defined result, not a crash. Phase 6 decides each edge case's final behaviour. |
| D12 | **Deterministic.** The same inputs always give the same schedule. Nothing is random, and nothing depends on the ordering of unordered collections. | Phase 21 checks the API returns the same schedule as the engine, and tests compare whole schedules. |
| D13 | **The engine never reads the clock.** Nothing in the scheduling library asks for the current date or time. | Carried from phase 3. "Today" is passed in when phase 8 needs it. |
| D14 | **Where things live.** The selection, session and schedule are domain types and sit with the phase 2 and 3 types. The scheduler is behaviour, not a domain type, and sits in its own part of the library. Tests mirror the same split. | Phases 5, 7 and 8 each change the scheduler, so keeping it apart from the domain types keeps those changes small. |
| D15 | **Everything is immutable,** and every list or set the engine hands out is read-only. | Carried from phase 2. The engine is a pure function. |

## Worked example

This is the example the plan's main test uses. A 1-week window from Monday 2 November 2026, with phase 3's week: Monday to Thursday 2 hours, Friday 1.5, Saturday 4, Sunday 0. That is 13.5 hours, or 27 half-hours.

- Maths (given first, Board A): sub-topics M1 and M2, 5 hours each. It needs 10 hours.
- English (Board B): sub-topics E1 (4 hours), E2 (3 hours) and E3 (3 hours). It needs 10 hours.

Shares (D6): 27 half-hours don't split evenly across 2 subjects, so Maths gets 14 half-hours (7 hours) and English 13 (6.5 hours). Neither gets all it needs. Inside subjects (D7): M1 and M2 get 3.5 hours each; E1 gets 2.5 hours, and E2 and E3 get 2 hours each.

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

Unused hours: none.

## Constraints

- The scheduling library keeps zero package references and zero project references.
- No enumerations are declared, and days of the week are never listed Sunday-first (phase 3).
- Every test follows Arrange, Act, Assert, with comment markers (tech-stack.md).
- The build has zero warnings, lint passes, and CI is green.
- Nothing names a real country, stage, board or subject. Subjects, stages and boards in tests are made-up data.
- Nothing identifies a student. The inputs are content, dates and hours only.

## Risks

- **Building phase 5 early.** It is tempting to weight sub-topics by size now. Don't: D7 splits evenly, and phase 5's tests are what change it. Keep the three steps, sharing across subjects (D6), splitting inside a subject (D7) and placing on dates (D8), separate in the code, so phases 5 and 7 can each replace one.
- **Rounding drift.** Splitting in fractions and rounding at the end can lose or gain half an hour. Working in whole half-hours (D1) avoids this; tests check that every subject's sessions add up to its share and every day's sessions to no more than its hours.
- **"Even share" tested only where it's easy.** Tests cover an uneven split (a spare half-hour), a subject that needs less than its share, and the lower limit in D6, not only two equal subjects.
- **Hidden order dependence.** Grouping with unordered collections can change the order of sessions between runs. D12 is checked by building the same schedule twice and by a test that gives sub-topic ids out of syllabus order.
- **The proposed decisions change in review.** D6, D7 and D8 are the parts most likely to change. The worked example and the tests that depend on it are rewritten if they do.

## Open questions for the review

1. **D6's lower limit.** Keep "at least one half-hour per selected sub-topic", or use a pure even split and fail when a subject can't fit its sub-topics?
2. **D8's placing.** Place work as early as possible (spare time at the end), or spread it across the whole window (spare time scattered)? Early placing leaves room for recap and for catching up after a missed day.
