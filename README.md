# Kotsu

> [!IMPORTANT]
> **Your data stays on your device.** Everything a student enters about their exams, and the schedule Kotsu makes, is stored only on their device. There are no accounts, Kotsu never asks for a name, and its servers keep nothing about the student. [More below](#your-data-stays-on-your-device).

Kotsu is a study scheduler for students. A student picks their country, stage and subjects, selects the syllabus topics they want to cover, and Kotsu spreads those topics across a 1 to 4 week window, sharing their study time across subjects.

> **Status:** building the scheduling engine. Phases 1 to 3 are done: the solution and CI, the domain model, and the study calendar that turns weekly study hours into the hours available on each date. Phase 4 is next. The plan lives in [specs/](specs/).

## What it does

1. The student chooses how long to schedule for (1 to 4 weeks) and their country: England, Wales or Scotland.
2. They choose their stage and their subjects. In England they also choose their exam board; in Scotland (SQA) and Wales (WJEC) the board is set automatically.
3. For each subject they select the whole syllabus or specific sub-topics.
4. They enter how many hours they can study on each day of the week.
5. Kotsu produces one schedule covering all their subjects, with recap time built in.

If the student misses days, Kotsu asks whether to reschedule and only does so if they say yes. If the window is too short to fit recap, Kotsu reminds them to recap in their own time.

## Your data stays on your device

> **Everything a student enters about their exams is stored only on their device.** Nothing about the student is kept on Kotsu's servers.

This covers:

- the schedule window, country and stage
- the subjects, exam board and chosen topics
- the study hours for each day of the week
- the schedule Kotsu produces, and which topics are marked complete

There are no accounts and no sign-in, and Kotsu never asks for the student's name. To work out or change a schedule, the app sends these details to Kotsu's server, which sends the schedule back and keeps none of it. A schedule doesn't move from one device to another, and clearing the device's storage removes it.

## Who it is for

Students aged 13 and up. Every screen must be understandable by a 13-year-old without help.

## Launch scope

Great Britain only. Northern Ireland and other countries follow after launch.

| Country | Stages | Exam board |
| --- | --- | --- |
| England | Key Stage 4, Key Stage 5 | Chosen by the student |
| Scotland | National 5, Higher, Advanced Higher | SQA |
| Wales | Key Stage 4 (GCSE), Post-16 (AS and A level) | WJEC |

Syllabus content is curated by the team and checked against each board's published specification.

## Platforms

Web, iOS and Android, all sharing one backend. Each device keeps its own schedule.

## Tech stack

| Area | Choice |
| --- | --- |
| Backend | .NET 11 (C#), ASP.NET Core, PostgreSQL with EF Core (syllabus content only) |
| Student data | Kept on the device; no accounts or sign-in |
| Scheduling engine | Pure C# library with no I/O |
| Web | React + Vite, TypeScript |
| Mobile | React Native + Expo, TypeScript, one codebase for iOS and Android |
| API client | TypeScript, generated from the backend's OpenAPI document |
| Hosting | Microsoft Azure |
| Observability | OpenTelemetry |
| Load | At least 10 requests per second normally, up to 50 at peak near exams |

Code is written test-first, and GitHub Actions runs tests and lint on every pull request.

## Build and test

You need the .NET SDK pinned in [global.json](global.json) (.NET 11). From the repository root:

```
dotnet build
dotnet format --verify-no-changes
dotnet test
```

The build treats analyzer warnings as errors, and `dotnet format` is the lint step CI runs.

## Documentation

- [Mission](specs/mission.md): what Kotsu does, for whom, and the product decisions.
- [Tech stack](specs/tech-stack.md): technology, architecture and development practice.
- [Roadmap](specs/roadmap.md): the phased implementation plan.
