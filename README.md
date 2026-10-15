# Kotsu

Kotsu is a syllabus scheduler for students. A student picks the exam board their school follows and the exam they are preparing for, and Kotsu schedules the syllabus content across the time remaining before the exam.

> **Status:** early planning. No code has been written yet; this README records the stakeholder input the project starts from.

## What it does

- **Choose an exam board:** the student selects the exam board their school follows.
- **Choose an exam:** the student selects which exam they need to schedule for.
- **Get a study schedule:** Kotsu spreads the syllabus content for that exam over the time remaining, so the student can prepare in time.

## Who it is for

Students aged 13 and up. The application must be very easy to understand and use for anyone in that age range.

## Platforms

| Client | Notes |
| --- | --- |
| Web | Browser-based interface |
| iOS | Separate mobile client |
| Android | Separate mobile client |

All three clients share a common backend.

## Stakeholder requirements

| Area | Requirement |
| --- | --- |
| Frontend | Modern TypeScript |
| Backend | Latest .NET, shared by the web and mobile clients |
| Scalability | Must be able to handle any load |
| Usability | Very easy to understand and use for ages 10+ |

## Open questions

These are not covered by the stakeholder input and need a decision before development starts:

- Which exam boards and exams are supported at launch, and where the syllabus content comes from.
- How the mobile clients are built (native per platform or a shared cross-platform codebase).
- What "any load" means as a measurable target (expected users, peak usage around exam season).
- Whether students need accounts, and what that means for the privacy of users under 18.
- Whether launch schedules one exam at a time or several overlapping exams sharing the same study hours.
