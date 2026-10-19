# Phase 2 — Plan

Branch: `feature/phase-2`, from `main` after Phase 1 was merged. See [requirement.md](requirement.md) for the decisions (D1–D15) and [validation.md](validation.md) for the merge criteria.

Every group is test-first. Write the failing tests for its rules, run them and see them fail, then write the least code that passes, then tidy up. Tests use Arrange, Act, Assert with comment markers and assert on error `Code`s (D2). Commit at the end of each group.

## 1. Clean up the scaffold

1.1 Delete `ScaffoldCheck.cs` and `ScaffoldCheckTests.cs`.
1.2 Create the `Domain/` folders in both projects (D15).

## 2. Result and ValidationError

2.1 Tests: a success holds a value and no errors. A failure holds one or more errors and no value. A failure can't be made with zero errors. Errors from several failures can be combined into one failure.
2.2 Implement `ValidationError(Code, Message)` and `Result<T>` (D1, D2), keeping them minimal.

## 3. Typed ids

3.1 Tests for code rules (D5): accepted examples are `scotland`, `sqa-national-5` and `ks4`. Rejected examples are an empty code, whitespace, uppercase, spaces, a leading or trailing `-`, other symbols, and 65 characters. Two ids with the same code are equal.
3.2 Implement the rules once and share them across `CountryId`, `StageId`, `SubjectId`, `BoardId`, `TopicId` and `SubTopicId`. Test each id type's `Create` at least once.

## 4. StudyHours

4.1 Tests (D3, D4): 0, 0.5, 1 and 1.5 are accepted. A negative value or 1.25 is rejected. Hours can be added together.
4.2 Implement `StudyHours`.

## 5. Content: board, stage, subject, country

5.1 Tests for name rules (D10): a blank name, a name with leading or trailing spaces, and a 101-character name are rejected.
5.2 `ExamBoard`, `QualificationStage` and `Subject`: each has an id and a name.
5.3 `Country` tests: it needs at least one stage and at least one board, and duplicate stage or board ids are rejected. `RequiresBoardChoice` is true only with two or more boards (D7).
5.4 Implement the four types.

## 6. Content: syllabus, topic, sub-topic

6.1 `SubTopic` tests: its estimated hours must be at least 0.5 (D4).
6.2 `Topic` tests: it needs at least one sub-topic, and duplicate sub-topic ids are rejected. `EstimatedHours` is the sum of its sub-topics' hours (D9).
6.3 `Syllabus` tests: it is identified by stage, subject and board. It needs at least one topic. Duplicate topic ids are rejected, and so is the same sub-topic id in two topics (D8, D10).
6.4 Implement the three types.

## 7. ScheduleWindow

7.1 Tests (D11): 1, 2, 3 and 4 weeks are accepted, and 0, 5 and a negative number are rejected. `EndDate` is inclusive, so a 1-week window from Monday 2 November 2026 ends on Sunday 8 November 2026. A window that crosses a month or year boundary works.
7.2 Implement `ScheduleWindow`.

## 8. WeeklyAvailability

8.1 Tests (D12): a value is needed for each of the 7 days, and a missing day is rejected. A negative value, 24.5 or 1.25 is rejected. All zeros is accepted. The hours for a given `DayOfWeek` can be read back.
8.2 Implement `WeeklyAvailability`.

## 9. Progress

9.1 Tests (D13): progress can be empty. Duplicates are merged. `IsComplete(subTopicId)` is true only for listed ids.
9.2 Implement `Progress`.

## 10. Great Britain fixture

10.1 Add a test that builds England (KS4 and KS5, boards AQA, Pearson Edexcel and OCR), Scotland (National 5, Higher and Advanced Higher, board SQA) and Wales (KS4 GCSE and Post-16 AS and A level, board WJEC) using only `Create` calls on the domain types. The test asserts that all three succeed, and that `RequiresBoardChoice` is true for England and false for Scotland and Wales.
10.2 Add one small syllabus for a Scottish National 5 subject with a few topics and sub-topics, and assert its derived topic hours. This shows a non-Key Stage stage carries real content.
10.3 The fixture lives in the test project only, as illustrative data, not as curated content (phase 14).

## 11. Review and PR

11.1 Check that `Kotsu.Scheduling` has no `enum` and no constants naming a country, stage or board (D6). Check that it still has no package or project references.
11.2 Make sure the build has 0 warnings, `dotnet format --verify-no-changes` passes, and `dotnet test` is green locally.
11.3 Push and open a PR to `main`, then confirm CI's `build-test-lint` is green.
11.4 Fill in the evidence section of [validation.md](validation.md).
