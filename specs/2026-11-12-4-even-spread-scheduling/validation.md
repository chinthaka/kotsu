# Phase 4 — Validation

The PR for `feature/phase-4` can merge when every check below passes and the evidence section is filled in.

## Roadmap "Done when"

- [ ] Every selected topic is scheduled exactly once: each selected sub-topic has sessions on consecutive study dates only, with no other sub-topic of its subject in between, and no unselected sub-topic is scheduled (D9). Shown by the invariant helper over every input in plan section 7.2.
- [ ] Every session is inside the window, on a date with hours, shown by the same helper.
- [ ] No day is given more work than its hours, shown by the same helper and by the worked example.
- [ ] Each subject gets an even share (D6), including when the share doesn't split into whole half-hours (plan 4.1).
- [ ] Hours a subject does not need go to the others (D6, plan 4.2 and 4.3), and hours no subject needs are reported in `UnusedHours` (D10, plan 4.4).

## Design

- [ ] `SubjectSelection` selects the whole syllabus or specific sub-topics, keeps syllabus order, merges repeats, and rejects an empty list or an unknown id (D2–D4).
- [ ] `Build` reports every broken schedule rule: no subjects, duplicate subject and mixed stage; different subjects with different boards are accepted (D5).
- [ ] Too little time, including no hours at all, gives `schedule.not-enough-hours` and never an exception (D11).
- [ ] Inside a subject, time is split evenly across sub-topics, with spare slots to the earliest (D7).
- [ ] Dates are filled in order, each date is shared across subjects in turn, and the first subject moves along each date (D8). The worked example's sessions match the table in [requirement.md](requirement.md) exactly.
- [ ] All hours are whole half-hours (D1), and `Schedule`, `StudySession` and `SubjectSelection` are immutable with read-only collections (D15).
- [ ] The same inputs give the same schedule (D12).
- [ ] The share, split and place steps are separate, so phases 5 and 7 can each replace one (code review).

## Tests and build

- [ ] Every test follows Arrange, Act, Assert with comment markers (code review).
- [ ] `Kotsu.Scheduling` has no `PackageReference`, no `ProjectReference`, no `enum` declarations, no `Enum.GetValues<DayOfWeek>()` and no clock reads (D13).
- [ ] `dotnet build` gives 0 warnings and 0 errors. `dotnet format --verify-no-changes` exits 0. `dotnet test` passes.
- [ ] CI's `build-test-lint` is green on the PR.

## Evidence

Fill this in before you mark the PR ready.

| Item | Link or output |
| --- | --- |
| PR | |
| Green CI run | |
| Local `dotnet test` summary line | |
| Searches in `backend/Kotsu.Scheduling` (expected: no matches) | |
| Tests catch real bugs (plan 8.2): which tests failed for each change | |
