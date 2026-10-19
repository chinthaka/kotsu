# Roadmap

High-level implementation order. The scheduling engine comes first: it is built and tested as a pure library before any API or UI exists.

Each phase is deliberately small, ends in something that can be demonstrated or tested, and is finished before the next begins.

Phases are built test-first (TDD): a phase starts by turning its "Done when" into failing tests, and is finished when they pass. The exceptions are listed in [tech-stack.md](tech-stack.md).

## Stage 1: Scheduling engine

Phases 3–12 schedule multiple subjects at once: selected topics across the student's subjects, placed within a 1–4 week window, sharing the weekly hours. The engine works in hours from phase 3: each sub-topic has an estimated study time, and each day has the hours the student can study.

| Phase | Work | Done when |
| --- | --- | --- |
| 1 | Create the GitHub repository with a GitHub Actions workflow that tests and lints every pull request, the .NET solution, the empty `Kotsu.Scheduling` library and its test project | `dotnet test` runs: one deliberately failing test is seen to fail, then made to pass |
| 2 | Model the domain: country, qualification stage, subject, exam board, syllabus topic, sub-topic (with its estimated study hours), schedule window, weekly study availability (hours for each of the 7 days), and progress (which scheduled topics are complete) | Types exist with tests for their validation rules; qualification stage is generic content per country (must fit SQA National 5/Highers, not a KS-only enum) |
| 3 | Study availability: turn the window and the hours for each day of the week into the hours available on each date | Every date in the window has exactly the hours entered for its weekday; a day with zero hours has none |
| 4 | Even-spread scheduling: place every selected topic into the available hours, splitting the hours evenly across subjects | Every selected topic is scheduled exactly once, inside the window; no day is given more work than its hours; each subject gets an even share, and hours a subject does not need go to the others |
| 5 | Topic weighting: time given to each topic is proportional to its estimated study hours | A topic twice the size gets about twice the time |
| 6 | Edge cases: window is a single day, more topics than days, no topics selected, no study hours at all | Each case returns a defined result, never an exception |
| 7 | Subject priority: split the shared hours across subjects by the agreed priority rule, replacing the even split | Subjects get hours according to the rule, and every phase 4–6 test still passes |
| 8 | Rescheduling: recompute the remaining topics from today, keeping completed ones | Completed topics stay put; the rest fit the time left |
| 9 | Missed-day detection: given a schedule, the completed topics and today's date, report whether the student has fallen behind | A schedule with unfinished work on past days is reported as "behind", with the missed days listed |
| 10 | Reschedule choice: apply the student's answer to "reschedule?" | "Yes" reschedules all unfinished work from today (phase 8); "No" returns the schedule unchanged |
| 11 | Recap time: reserve recap sessions inside the schedule, alongside new topics | Every schedule with enough time contains recap sessions, and all topics still finish inside the window |
| 12 | Tight time: when there is too little time in the window to fit recap sessions, leave them out and flag it | The schedule has no recap sessions and carries a "recap in your own time" flag |

## Stage 2: Content

| Phase | Work | Done when |
| --- | --- | --- |
| 13 | Define the seed data format for countries, stages, subjects, boards, topics and sub-topics, including each sub-topic's estimated study hours | One hand-written sample file loads and validates |
| 14 | Curate one subject for one stage end to end | The engine produces a sensible schedule from real content |
| 15 | Curate the remaining launch stages, subjects and boards | Every launch exam loads, validates and schedules |

## Stage 3: Backend API

| Phase | Work | Done when |
| --- | --- | --- |
| 16 | Create `Kotsu.Api` with a health endpoint, instrumented with OpenTelemetry | A request to the health endpoint returns OK and produces a trace |
| 17 | Deploy the API to Azure from GitHub Actions, with infrastructure defined in the repository | A merge to the main branch deploys, and the health endpoint answers on an Azure URL |
| 18 | Database, EF Core model and migrations; load seed content | Content is in PostgreSQL after a clean setup |
| 19 | Read endpoints: list countries, stages, subjects, boards and topics | Endpoints return seed content and appear in the OpenAPI document |
| 20 | Schedule endpoint: subjects, selected topics, window and weekly hours in, schedule out (no accounts yet) | The API returns the same schedule the engine does, including recap sessions or the recap flag |
| 21 | Authentication: sign-up and sign-in, meeting the UK Children's Code | Only a signed-in student can reach protected endpoints |
| 22 | Save and fetch a student's schedule | A schedule survives sign-out and sign-in |
| 23 | Mark a topic complete | Progress is stored |
| 24 | Missed days: report when the student is behind, and accept their reschedule answer | "Yes" stores the rescheduled plan; "No" leaves the stored plan as it is |

## Stage 4: Web client

| Phase | Work | Done when |
| --- | --- | --- |
| 25 | Generate the TypeScript API client; create the React + Vite app | The app builds and calls the health endpoint |
| 26 | Sign-up and sign-in screens | A student can create an account and sign in |
| 27 | Enter name, schedule window and country; choose stage | The student sets 1–4 weeks, picks England, Wales or Scotland, and picks a stage |
| 28 | Choose subjects, choose board (England) and select topics | The student picks subjects, picks a board in England (automatic elsewhere), and selects the whole syllabus or specific sub-topics |
| 29 | Enter study hours for each day of the week | The student sets hours for all 7 days |
| 30 | Show the schedule, including recap sessions | The student sees what to study and what to recap, day by day |
| 31 | Recap reminder | When the schedule has no room for recap, the student is reminded to recap in their own time |
| 32 | Mark topics complete | Progress shows on the schedule |
| 33 | Missed-day prompt: ask the student whether to reschedule | "Yes" shows the new schedule; "No" keeps the current one |
| 34 | Usability pass with students aged 13 and up | Test students finish the flow without help |

## Stage 5: Mobile clients

| Phase | Work | Done when |
| --- | --- | --- |
| 35 | Create the Expo app using the shared API client | The app runs on iOS and Android and calls the API |
| 36 | Sign-up and sign-in | A student signs in on a phone |
| 37 | Choose country, stage, subjects and topics, and enter study hours | Same flow as web, on mobile |
| 38 | Show the schedule with recap sessions, and mark topics complete | A schedule made on web appears on mobile, and the reverse |
| 39 | Missed-day prompt and recap reminder | Same behaviour as web, on mobile |
| 40 | Store builds | Test builds are installable from TestFlight and Google Play testing |

## Stage 6: Scale and launch

| Phase | Work | Done when |
| --- | --- | --- |
| 41 | Write a k6 load test, kept in the repository, with two profiles across the main endpoints: normal (10 requests per second) and peak (50) | Both profiles run against a deployed environment and report response times against the target (95% within 500 ms) |
| 42 | HTTP caching for board, exam and syllabus content | Repeat content requests are served from cache |
| 43 | Run the load test against the launch architecture and fix what breaks | 10 requests per second is sustained, and the 50 requests per second peak is handled, both within the response-time target |
| 44 | Dashboards and alerts on the OpenTelemetry data | A failure in production pages someone |
| 45 | Privacy and accessibility review | UK GDPR, Children's Code, data retention and accessibility are signed off |
| 46 | Great Britain launch | Web is live and both apps are in the stores |

## Stage 7: Other countries

Not planned in detail until the Great Britain launch is done. Each new country is its own small set of phases:

| Phase | Work | Done when |
| --- | --- | --- |
| 47 | Choose the next country and its stages, subjects and boards | The list is written into [mission.md](mission.md) |
| 48 | Review that country's privacy and consent rules for minors | Any change to sign-up is identified |
| 49 | Curate its syllabus content, and translate the clients if needed | Every exam loads, validates and schedules |
| 50 | Release in that country | Students there can sign up and get a schedule |

## Decisions that block phases

These are open in [mission.md](mission.md) and [tech-stack.md](tech-stack.md). Each must be settled before the phase shown.

| Decision | Needed before |
| --- | --- |
| How shared weekly hours are split across subjects (priority rule); phases 4–6 use an even split until then | Phase 7 |
| What counts as "behind", and what happens after the student answers "No" | Phase 9 |
| How much recap time a schedule reserves, and how little time counts as too little | Phase 11 |
| Named list of England launch boards and launch subjects per stage | Phase 14 |
| Sign-in method, authentication provider and Children's Code review | Phase 21 |
