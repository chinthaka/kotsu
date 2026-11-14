<!--
Template for a phase's plan.md.
Copy this file into specs/YYYY-MM-DD-<phase-number>-<short-name>/ and replace every <placeholder>.
Write in plain, simple English. Say what each step must do and how it is checked,
not how to code it: no names of types, methods, files, test classes or commands.
Put every numbered step (1.1, 1.2, ...) on its own line, with a blank line between steps.
Delete these comments when the spec is ready for review.
-->

# Phase <phase-number> — Plan

Branch: `features/YYYY-MM-DD-<phase-number>-<short-name>`, from `main` after Phase <previous-phase-number> was merged. See [requirement.md](requirement.md) for the decisions (D1–D<n>) and [validation.md](validation.md) for the merge criteria and the test coverage.

Every section is test-first. Write the failing tests for its rules, run them and see them fail, then write the least code that passes, then tidy up. Tests use Arrange, Act, Assert with comment markers. Commit at the end of each section.

How the code is shaped, and what things are called, is up to the developers. This plan says what each step must do and how it is checked.

<The default inputs the tests use unless a test says otherwise, such as a window, a week of hours, or made-up subjects.>

## 1. <First piece of work>

1.1 Test (D<n>): <the input, and what must happen>.

1.2 Tests (D<n>): <the input, and what must happen>.

1.3 Implement <the piece of work, in plain words>.

## 2. <Next piece of work>

2.1 Test (D<n>): <the input, and what must happen>.

2.2 Implement <the piece of work>.

## <n>. The "Done when" checks

<!-- Turn each part of the roadmap's "Done when" into a check, and run it over a range of inputs. -->

<n>.1 <The checks that must hold for every result, one bullet each:>
- <check>
- <check>

<n>.2 Run these checks over at least these inputs: <list>.

<n>.3 Fix anything the tests turn up, then tidy up.

## <n+1>. Review and PR

<n+1>.1 Check the code for the things the constraints forbid:
- <forbidden thing, in plain words>

<n+1>.2 Check the tests catch real bugs. Make each change below, see at least one test fail, then revert:
- <a deliberate bug>

<n+1>.3 Make sure the build has 0 warnings, the formatting check passes, and all tests pass locally.

<n+1>.4 Push and open a PR to `main`, then confirm CI is green.

<n+1>.5 Fill in the evidence section of [validation.md](validation.md).
