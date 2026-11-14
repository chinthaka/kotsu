<!--
Template for a phase's requirement.md.
Copy this file into specs/YYYY-MM-DD-<phase-number>-<short-name>/ and replace every <placeholder>.
Write in plain, simple English. Describe what must happen, not how to code it:
no names of types, methods, files, test classes or error codes. Those are left to the developers.
Delete these comments when the spec is ready for review.
-->

# Phase <phase-number> — Requirements

Roadmap phase <phase-number> ([roadmap.md](../roadmap.md)), Stage <stage-number>: <stage name>.

This document describes what must be built, not how to code it. Names of types, methods, files and error codes are left to the developers, who follow the conventions set in earlier phases.

## Goal

<The roadmap's "Work" for this phase, copied word for word.>

**Done when** (from the roadmap): <the roadmap's "Done when" for this phase, copied word for word>.

## Context

<!--
What is already true before this phase starts, and why it matters here. One bullet each for:
- the earlier phases this one builds on, and what they already give;
- anything an earlier phase left for this one to do;
- the lines in mission.md and tech-stack.md that shape this phase;
- the later phases that depend on this one, and what they will expect from it.
-->

- Phase <n> is merged. <What it gives this phase, in plain words.>
- [mission.md](../mission.md): <the rule or decision that applies>.
- [tech-stack.md](../tech-stack.md): <the rule or decision that applies>.
- Phase <n> <depends on / replaces> this work: <what it will expect>.

## Scope

In scope:

1. <Each thing this phase builds, described by what it is and what it holds, not by its code name.>
2. Tests that turn the roadmap's "Done when" into checks.

Out of scope:

- <Each nearby thing this phase does not do, with the phase that does it.> (phase <n>)

## Decisions

Decisions marked **Proposed** need confirming in the phase <phase-number> review.

| # | Decision | Notes |
| --- | --- | --- |
| D1 | **<Short bold summary.>** <The decision in plain words.> | <Why, and which earlier decision or document it follows.> |
| D2 | **<Short bold summary>** (**Proposed**). <The decision.> | <Why, and the alternative that was considered.> |

## Worked example

<!--
Optional, but strongly recommended for any phase that computes something.
A small, hand-checked example with every input and the exact expected result.
The plan's main test uses it, so check the numbers twice.
-->

<Inputs, step-by-step working, and the expected result, using a table where it helps.>

## Constraints

<!-- Rules carried from the constitution and earlier phases that the work must not break. -->

- Every test follows Arrange, Act, Assert, with comment markers (tech-stack.md).
- The build has zero warnings, lint passes, and CI is green.
- Nothing assumes Great Britain, and nothing identifies a student.
- <Any other constraint.>

## Risks

- **<Risk in a few words.>** <What could go wrong, and how the plan or tests guard against it.>

## Open questions for the review

<!-- Questions the reviewer must answer before work starts. Remove this section if there are none. -->

1. **<Decision it affects.>** <The question, and the options.>
