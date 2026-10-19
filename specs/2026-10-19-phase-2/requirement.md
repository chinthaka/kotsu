# Phase 2 — Requirements

Roadmap phase 2 ([roadmap.md](../roadmap.md)), Stage 1: Scheduling engine.

## Goal

Model the domain that the scheduling engine works on: country, qualification stage, subject, exam board, syllabus topic, sub-topic (with its estimated study hours), schedule window, weekly study availability (hours for each of the 7 days), and progress (which scheduled topics are complete).

**Done when** (from the roadmap): the types exist with tests for their validation rules, and the qualification stage is generic content per country. It must fit SQA National 5 and Highers, so it cannot be a Key Stage-only enum.

## Context

- Phase 1 is merged. `Kotsu.Scheduling` is an empty `net11.0` library with an xUnit test project, and CI builds, lints and tests every PR.
- [tech-stack.md](../tech-stack.md): the engine is a pure C# library with no I/O, database or web dependencies. Country is data, not code, and there is no KS-specific enum. Schedule windows are calendar dates. Work is test-first, and every test follows Arrange, Act, Assert.
- [mission.md](../mission.md): the launch countries are England (KS4, KS5, board chosen by the student), Scotland (National 5, Higher, Advanced Higher, SQA) and Wales (KS4 GCSE, Post-16 AS and A level, WJEC). The window is 1 to 4 weeks. The student enters hours for each of the 7 days, and every sub-topic carries an estimated study time in hours.
- Later phases depend on these types. Phase 3 turns the window and weekly hours into hours per date. Phases 4–5 schedule selected sub-topics by their hours. Phase 6 needs valid edge cases (no topics, zero hours) to be representable. Phase 13 loads the same content from hand-written seed files.

## Scope

In scope:

1. A `Result<T>` and `ValidationError` used by every type's `Create` factory.
2. Typed ids: `CountryId`, `StageId`, `SubjectId`, `BoardId`, `TopicId` and `SubTopicId`.
3. `StudyHours`, the unit of study time.
4. Content types: `Country`, `QualificationStage`, `ExamBoard`, `Subject`, `Syllabus`, `Topic` and `SubTopic`.
5. Plan types: `ScheduleWindow`, `WeeklyAvailability` and `Progress`.
6. Tests for every validation rule, and a fixture test that builds all three Great Britain launch countries as data.
7. Removing the phase 1 `ScaffoldCheck` and its test.

Out of scope:

- The student's topic selection (whole syllabus or specific sub-topics). Phase 4 introduces it together with scheduling.
- Hours per date (phase 3) and any scheduling logic (phase 4 onwards).
- Recap sessions and the recap flag (phases 11–12).
- Loading content from files (phase 13), real curated content (phase 14), persistence and the API.
- The student's account and first name. They belong to the API (phase 21 onwards), not the engine.

## Decisions

| # | Decision | Notes |
| --- | --- | --- |
| D1 | **Validated types are created through `Create(...)`, which returns a `Result<T>`.** Constructors are private, and `Create` never throws for invalid input. | This was chosen in the phase 2 review. An invalid object can't exist, and the API (phase 20) can return the errors to the client as they are. |
| D2 | `Result<T>` holds either a value or a non-empty list of `ValidationError`s. `ValidationError` has a stable `Code` (for example `study-hours.not-half-hour-step`) and a human-readable `Message`. `Create` reports **every** broken rule, not only the first. | Tests assert on `Code`, never on `Message`, so the wording can change freely. |
| D3 | **Study hours come in half-hour steps.** `StudyHours` wraps a `decimal` that is zero or more and a multiple of 0.5. | This was chosen in the phase 2 review. `decimal` keeps 0.5 steps exact, where `double` would add rounding errors. |
| D4 | A sub-topic's estimated hours must be at least 0.5. A day's available hours must be from 0 to 24. | Zero hours is valid for a day (a rest day, or the phase 6 "no study hours at all" case), but not for a sub-topic. |
| D5 | **Typed string-code ids.** Each id is a `readonly record struct` wrapping a code, such as `CountryId("scotland")` or `StageId("sqa-national-5")`. | This was chosen in the phase 2 review. A code is 1–64 characters of lowercase `a–z`, `0–9` and `-`, and doesn't start or end with `-` or contain `--`. Ids are created through `Create` (D1). |
| D6 | **Qualification stages are data.** `Country` holds its own list of `QualificationStage`s and `ExamBoard`s. There is no enum or constant anywhere in `Kotsu.Scheduling` that names a country, stage or board. | This is the roadmap's "Done when" and tech-stack.md's "Country is data, not code". |
| D7 | `Country.RequiresBoardChoice` is true when the country has more than one board. | This gives England "chosen by the student", and Scotland and Wales "set automatically", with no special-casing. |
| D8 | **`Syllabus` links content.** A syllabus is identified by a (stage, subject, board) triple and holds that combination's `Topic`s. Each `Topic` holds its `SubTopic`s. | The same subject has different content per board in England and per stage everywhere, so topics hang off the combination, not off the subject. `Syllabus` isn't on the roadmap's list, but it is the "syllabus" part of "syllabus topic". |
| D9 | A topic's hours are the sum of its sub-topics' hours. They are derived and never stored. | One source of truth for phase 5's weighting. |
| D10 | Content rules: names are non-blank, trimmed, and at most 100 characters. A country has at least one stage and at least one board. A syllabus has at least one topic, and a topic has at least one sub-topic. Ids are unique within each list. | |
| D11 | `ScheduleWindow` has a `StartDate` (`DateOnly`) and a number of `Weeks` from 1 to 4. `EndDate` is derived and inclusive: `StartDate + 7 × Weeks − 1` days. | The mission says the student chooses "1 to 4 weeks", so whole weeks, not free date ranges. Calendar dates have no time zone (tech-stack.md). |
| D12 | `WeeklyAvailability` must have hours for all 7 `DayOfWeek` values, each a valid day amount (D4). All-zero is valid. | `DayOfWeek` is the BCL enum and names no country, stage or board, so it doesn't conflict with D6. |
| D13 | **Progress is tracked by sub-topic.** `Progress` is the set of completed `SubTopicId`s. Duplicates are merged, not rejected. | Sub-topics are the smallest thing a student selects and the engine schedules. Marking the same sub-topic complete twice is harmless. |
| D14 | All types are immutable, and collections are exposed as read-only lists or sets. | The engine is a pure function (tech-stack.md, Scalability). |
| D15 | Types live in the `Kotsu.Scheduling.Domain` namespace, in `backend/Kotsu.Scheduling/Domain/`. Tests mirror this in `backend/tests/Kotsu.Scheduling.Tests/Domain/`. | |

## Constraints

- `Kotsu.Scheduling` keeps zero package references and zero project references.
- Every test follows Arrange, Act, Assert, with comment markers (tech-stack.md).
- The build has zero warnings, lint passes, and CI is green.
- Nothing in the model may assume Great Britain. Names, stages and boards come only from data.

## Risks

- **Over-modelling ahead of need.** Keep each type to what the roadmap lists plus `Syllabus` (D8). Add fields only when a later phase's tests need them.
- **The `Result` boilerplate grows.** Keep `Result<T>` minimal: success or errors, and a way to combine errors from nested `Create` calls. Don't build a general functional library.
