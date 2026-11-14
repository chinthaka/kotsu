# Phase 4 — Validation

The PR for `features/2026-11-12-4-even-spread-scheduling` can merge when every check below passes and the evidence section is filled in.

## Roadmap "Done when"

- [ ] Every selected topic is scheduled exactly once: each selected sub-topic's sessions form one unbroken run within its subject, with no other sub-topic of its subject in between, and no unselected sub-topic is scheduled (D9). Shown by the invariant checks over every input in plan section 7.2.
- [ ] Every session is inside the window, on a date with hours, shown by the same checks.
- [ ] No day is given more work than its hours, shown by the same checks and by the worked example.
- [ ] Each subject gets an even share (D6), including when the share doesn't split into whole half-hours (plan 4.1).
- [ ] Hours a subject does not need go to the others (D6, plan 4.2 and 4.3), and hours no subject needs are reported as unused (D10, plan 4.4).

## Design

- [ ] A subject selection covers the whole syllabus or specific sub-topics, keeps syllabus order, merges repeats, and rejects an empty list or an unknown sub-topic (D2–D4).
- [ ] The scheduler reports every broken schedule rule, not only the first: no subjects, duplicate subject and mixed stage. Different subjects with different boards are accepted (D5).
- [ ] Too little time, including no hours at all, gives a reported failure and never a crash (D11).
- [ ] Inside a subject, time is split evenly across sub-topics, with spare half-hours to the earliest (D7).
- [ ] Dates are filled in order, each date is shared across subjects in turn, and the first subject moves along each date (D8). The worked example's sessions match the table in [requirement.md](requirement.md) exactly.
- [ ] All hours are whole half-hours (D1), and the selection, session and schedule are immutable with read-only collections (D15).
- [ ] The same inputs give the same schedule (D12).
- [ ] The sharing, splitting and placing steps are separate in the code, so phases 5 and 7 can each replace one (code review).

## Types of test

All tests in this phase are unit tests of the pure scheduling library. There is no I/O, database or API yet, so there are no integration or end-to-end tests. Within that, the plan uses four kinds:

| Kind | What it does | Where in the plan |
| --- | --- | --- |
| Rule tests | One input that breaks one rule, checked by the stable code of the problem reported. Also one input that breaks two rules at once, to show every problem is reported. | 1.4, 3.1–3.4 |
| Example tests | A small hand-worked input with an exact expected answer: the hours each subject gets, the hours each sub-topic gets, or the full list of sessions. | 4.1–4.5, 5.1–5.2, 6.1–6.4 |
| Invariant tests | One set of checks that must hold for any schedule, run over many different inputs. These are the direct translation of the roadmap's "Done when". | 7.1–7.3 |
| Mutation checks | Deliberate bugs put into the code by hand to prove the tests notice them, then reverted. Not kept as tests; recorded in the evidence. | 8.2 |

## Test coverage

What the tests cover, decision by decision:

| Area | Covered by |
| --- | --- |
| Half-hour arithmetic (D1) | Every example test uses amounts that end in .5, and the invariant checks confirm totals add up exactly. |
| Selection: whole or specific, order, merged repeats, rejections (D2–D4) | Section 1. |
| Schedule rules: empty, duplicate subject, mixed stage, mixed boards accepted, several problems at once (D5) | 3.1–3.3. |
| Even share, spare half-hour to the first subject, order matters (D6) | 4.1. |
| Unneeded hours go to the others, with two and with three subjects (D6) | 4.2, 4.3. |
| Everyone's need met, unused hours reported (D6, D10) | 4.4, 6.2. |
| Lower limit of one half-hour per sub-topic (D6) | 4.5. |
| Even split inside a subject, spare to the earliest (D7) | 5.1, 5.2. |
| Placing: date order, turn-taking, first subject moves along, mid-week start, sub-topic carries over (D8, D9) | 6.1, 6.3 and the invariant checks. |
| Lookups by date and subject, zero outside the schedule (D10) | 6.4. |
| Not enough time, including no hours at all (D11) | 3.4. |
| Same inputs, same schedule (D12) | 7.3. |
| No clock reads, no enumerations, no unordered iteration, no references (D13, D15) | Code review and searches in 8.1. |
| Window lengths and shapes | 1 week in most tests, 4 weeks and mid-week start in 7.2, half-hour days in 7.2. |

## Missing tests, and why

These are deliberately not tested in phase 4:

- **Weighting by sub-topic size.** Phase 5 owns it. Test 5.2 shows the opposite on purpose and is expected to change then.
- **Each edge case's final behaviour** (a single-day window, more topics than days, no topics, no hours). Phase 6 owns them. Phase 4 only shows that no hours and too little time give a reported failure, not a crash.
- **Priority across subjects.** Phase 7 owns it.
- **Progress, rescheduling, missed days and recap.** Phases 8–12. The scheduler takes none of these inputs yet.
- **Randomised or property-based testing.** The invariant checks are run over a hand-picked list of inputs instead. Adding a property-testing library is a decision for the team, and the engine's inputs are small enough that a hand-picked list gives good coverage for now. If later phases keep adding invariants, revisit this.
- **Performance.** Windows are at most 4 weeks and content is small, so the engine's speed is not a concern until the load tests in Stage 6.
- **Real syllabus content.** Phase 14 schedules real content for the first time. Phase 4 uses made-up subjects, stages and boards so that nothing in the engine or its tests assumes Great Britain.
- **The wording of error messages.** Tests check the stable code only, so wording can change freely (phase 2).

## Tests and build

- [ ] Every test follows Arrange, Act, Assert with comment markers (code review).
- [ ] The scheduling library has no package references, no project references, no enumerations, no Sunday-first day listing and no clock reads (D13).
- [ ] The build gives 0 warnings and 0 errors. The formatting check passes. All tests pass.
- [ ] CI is green on the PR.

## Evidence

Fill this in before you mark the PR ready.

| Item | Link or output |
| --- | --- |
| PR | |
| Green CI run | |
| Local test run summary line | |
| Searches in the scheduling library (expected: no matches) | |
| Tests catch real bugs (plan 8.2): which tests failed for each change | |
| Test count by kind (rule, example, invariant) | |
