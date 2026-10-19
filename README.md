# Kotsu

Kotsu is a study scheduler for students. A student picks their country, stage and subjects, selects the syllabus topics they want to cover, and Kotsu spreads those topics across a 1 to 4 week window, sharing their study time across subjects.

> **Status:** planning. No application code has been written yet. The plan lives in [specs/](specs/).

## What it does

1. The student signs in.
2. They enter their first name (used only to greet them), how long to schedule for (1 to 4 weeks), and their country: England, Wales or Scotland.
3. They choose their stage and their subjects. In England they also choose their exam board; in Scotland (SQA) and Wales (WJEC) the board is set automatically.
4. For each subject they select the whole syllabus or specific sub-topics.
5. They enter how many hours they can study on each day of the week.
6. Kotsu produces one schedule covering all their subjects, with recap time built in.

If the student misses days, Kotsu asks whether to reschedule and only does so if they say yes. If the window is too short to fit recap, Kotsu reminds them to recap in their own time.

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

Web, iOS and Android, all sharing one backend.

## Tech stack

| Area | Choice |
| --- | --- |
| Backend | .NET 10 (C#), ASP.NET Core, PostgreSQL with EF Core |
| Scheduling engine | Pure C# library with no I/O |
| Web | React + Vite, TypeScript |
| Mobile | React Native + Expo, TypeScript, one codebase for iOS and Android |
| API client | TypeScript, generated from the backend's OpenAPI document |
| Hosting | Microsoft Azure |
| Observability | OpenTelemetry |
| Load | At least 10 requests per second normally, up to 50 at peak near exams |

Code is written test-first, and GitHub Actions runs tests and lint on every pull request.

## Documentation

- [Mission](specs/mission.md): what Kotsu does, for whom, and the product decisions.
- [Tech stack](specs/tech-stack.md): technology, architecture and development practice.
- [Roadmap](specs/roadmap.md): the phased implementation plan.
