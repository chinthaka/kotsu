# Roadmap

High-level implementation order. The scheduling engine comes first: it is built and tested as a pure library before any API or UI exists.

Each phase is deliberately small, ends in something that can be demonstrated or tested, and is finished before the next begins.

Phases are built test-first (TDD): a phase starts by turning its "Done when" into failing tests, and is finished when they pass. The exceptions are listed in [tech-stack.md](tech-stack.md).

## Stage 1: Scheduling engine

Phases 3–11 assume one exam per schedule until the multi-exam decision is made; the engine API must stay open to multiple exams sharing weekly hours.

| Phase | Work | Done when |
| --- | --- | --- |
| 1 | Create the GitHub repository with a GitHub Actions workflow that tests and lints every pull request, the .NET solution, the empty `Kotsu.Scheduling` library and its test project | `dotnet test` runs: one deliberately failing test is seen to fail, then made to pass |
| 2 | Model the domain: country, exam board, qualification level, exam, syllabus topic, exam date | Types exist with tests for their validation rules; qualification level is generic content (must fit SQA National 5/Highers, not a KS-only enum) |
| 3 | Even-spread scheduling: given topics, a start date and an exam date, assign every topic to a day | Every topic is scheduled exactly once, before the exam date |
| 4 | Topic weighting: larger topics get more time | A topic twice the size gets about twice the time |
| 5 | Edge cases: exam is tomorrow, exam has passed, more topics than days, no topics | Each case returns a defined result, never an exception |
| 6 | Study availability: use the hours the student can study on each of the 7 days of the week | No day is given more work than its hours; a day with zero hours gets none |
| 7 | Rescheduling: recompute the remaining topics from today, keeping completed ones | Completed topics stay put; the rest fit the time left |
| 8 | Missed-day detection: given a schedule, the completed topics and today's date, report whether the student has fallen behind | A schedule with unfinished work on past days is reported as "behind", with the missed days listed |
| 9 | Reschedule choice: apply the student's answer to "reschedule?" | "Yes" reschedules all unfinished work from today (phase 7); "No" returns the schedule unchanged |
| 10 | Recap time: reserve recap sessions inside the schedule, alongside new topics | Every schedule with enough time contains recap sessions, and all topics still finish before the exam |
| 11 | Tight time: when there is too little time before the exam to fit recap sessions, leave them out and flag it | The schedule has no recap sessions and carries a "recap in your own time" flag |

## Stage 2: Content

| Phase | Work | Done when |
| --- | --- | --- |
| 12 | Define the seed data format for boards, exams and topics | One hand-written sample file loads and validates |
| 13 | Curate one GCSE exam for one UK board, end to end | The engine produces a sensible schedule from real content |
| 14 | Curate the remaining UK launch boards and their KS4 and KS5 exams | Every launch exam loads, validates and schedules |

## Stage 3: Backend API

| Phase | Work | Done when |
| --- | --- | --- |
| 15 | Create `Kotsu.Api` with a health endpoint, instrumented with OpenTelemetry | A request to the health endpoint returns OK and produces a trace |
| 16 | Deploy the API to Azure from GitHub Actions, with infrastructure defined in the repository | A merge to the main branch deploys, and the health endpoint answers on an Azure URL |
| 17 | Database, EF Core model and migrations; load seed content | Content is in PostgreSQL after a clean setup |
| 18 | Read endpoints: list boards, list exams for a board | Endpoints return seed content and appear in the OpenAPI document |
| 19 | Schedule endpoint: exam, dates and weekly hours in, schedule out (no accounts yet) | The API returns the same schedule the engine does, including recap sessions or the recap flag |
| 20 | Authentication: sign-up and sign-in, meeting the UK Children's Code | Only a signed-in student can reach protected endpoints |
| 21 | Save and fetch a student's schedule | A schedule survives sign-out and sign-in |
| 22 | Mark a topic complete | Progress is stored |
| 23 | Missed days: report when the student is behind, and accept their reschedule answer | "Yes" stores the rescheduled plan; "No" leaves the stored plan as it is |

## Stage 4: Web client

| Phase | Work | Done when |
| --- | --- | --- |
| 24 | Generate the TypeScript API client; create the React + Vite app | The app builds and calls the health endpoint |
| 25 | Sign-up and sign-in screens | A student can create an account and sign in |
| 26 | Choose an exam board | The student picks a board from the list |
| 27 | Choose an exam and exam date | The student picks an exam for that board |
| 28 | Enter study hours for each day of the week | The student sets hours for all 7 days |
| 29 | Show the schedule, including recap sessions | The student sees what to study and what to recap, day by day |
| 30 | Recap reminder | When the schedule has no room for recap, the student is reminded to recap in their own time |
| 31 | Mark topics complete | Progress shows on the schedule |
| 32 | Missed-day prompt: ask the student whether to reschedule | "Yes" shows the new schedule; "No" keeps the current one |
| 33 | Usability pass with students aged 13 and up | Test students finish the flow without help |

## Stage 5: Mobile clients

| Phase | Work | Done when |
| --- | --- | --- |
| 34 | Create the Expo app using the shared API client | The app runs on iOS and Android and calls the API |
| 35 | Sign-up and sign-in | A student signs in on a phone |
| 36 | Choose board and exam, and enter study hours | Same flow as web, on mobile |
| 37 | Show the schedule with recap sessions, and mark topics complete | A schedule made on web appears on mobile, and the reverse |
| 38 | Missed-day prompt and recap reminder | Same behaviour as web, on mobile |
| 39 | Store builds | Test builds are installable from TestFlight and Google Play testing |

## Stage 6: Scale and launch

| Phase | Work | Done when |
| --- | --- | --- |
| 40 | Write a k6 load test, kept in the repository, with two profiles across the main endpoints: normal (10 requests per second) and peak (50) | Both profiles run against a deployed environment and report response times |
| 41 | HTTP caching for board, exam and syllabus content | Repeat content requests are served from cache |
| 42 | Run the load test against the launch architecture and fix what breaks | 10 requests per second is sustained, and the 50 requests per second peak is handled, both within the response-time target |
| 43 | Dashboards and alerts on the OpenTelemetry data | A failure in production pages someone |
| 44 | Privacy and accessibility review | UK GDPR, Children's Code, data retention and accessibility are signed off |
| 45 | UK launch | Web is live and both apps are in the UK stores |

## Stage 7: Other countries

Not planned in detail until the UK launch is done. Each new country is its own small set of phases:

| Phase | Work | Done when |
| --- | --- | --- |
| 46 | Choose the next country and its boards and exams | The list is written into [mission.md](mission.md) |
| 47 | Review that country's privacy and consent rules for minors | Any change to sign-up is identified |
| 48 | Curate its syllabus content, and translate the clients if needed | Every exam loads, validates and schedules |
| 49 | Release in that country | Students there can sign up and get a schedule |

## Decisions that block phases

These are open in [mission.md](mission.md) and [tech-stack.md](tech-stack.md). Each must be settled before the phase shown.

| Decision | Needed before |
| --- | --- |
| What counts as "behind", and what happens after the student answers "No" | Phase 8 |
| How much recap time a schedule reserves, and how little time counts as too little | Phase 10 |
| Whether Scotland (SQA, non-KS qualifications) is in scope – domain model depends on it | Phase 2 |
| Single-exam vs multi-exam schedules at launch (and multi-exam openness if single) | Phase 3 |
| Named list of UK launch boards and exams | Phase 13 |
| Sign-in method, authentication provider and Children's Code review | Phase 20 |
| Response-time target for the load test | Phase 40 |
