<!--
Template for a phase's validation.md.
Copy this file into specs/YYYY-MM-DD-<phase-number>-<short-name>/ and replace every <placeholder>.
Write in plain, simple English: no names of types, methods, files, test classes or commands.
Every checkbox should point to the decision (D<n>) and the plan step that proves it.
Delete these comments when the spec is ready for review.
-->

# Phase <phase-number> — Validation

The PR for `features/YYYY-MM-DD-<phase-number>-<short-name>` can merge when every check below passes and the evidence section is filled in.

## Roadmap "Done when"

<!-- One checkbox for each part of the roadmap's "Done when". -->

- [ ] <Part of the "Done when">, shown by <plan step>.

## Design

<!-- One checkbox for each decision, or group of related decisions, in requirement.md. -->

- [ ] <What must be true> (D<n>).

## Types of test

<Where the tests run, for example unit tests of a pure library, integration tests against a real database, or end-to-end tests in a browser, and which kinds are not needed yet and why.>

| Kind | What it does | Where in the plan |
| --- | --- | --- |
| <Kind, for example rule, example or invariant tests> | <What one test of this kind checks> | <plan steps> |
| Mutation checks | Deliberate bugs put into the code by hand to prove the tests notice them, then reverted. Not kept as tests; recorded in the evidence. | <plan step> |

## Test coverage

| Area | Covered by |
| --- | --- |
| <Behaviour or rule> (D<n>) | <plan steps> |

## Missing tests, and why

<!-- Everything a reader might expect to be tested here but isn't, each with the reason and, where it applies, the phase that will test it. -->

- **<What isn't tested.>** <Why not, and which phase owns it.>

## Tests and build

- [ ] Every test follows Arrange, Act, Assert with comment markers (code review).
- [ ] <Constraint checks from requirement.md, in plain words.>
- [ ] The build gives 0 warnings and 0 errors. The formatting check passes. All tests pass.
- [ ] CI is green on the PR.

## Evidence

Fill this in before you mark the PR ready.

| Item | Link or output |
| --- | --- |
| PR | |
| Green CI run | |
| Local test run summary line | |
| Searches for forbidden things (expected: no matches) | |
| Tests catch real bugs: which tests failed for each deliberate bug | |
| Test count by kind | |
