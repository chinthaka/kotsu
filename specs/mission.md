# Mission

## Purpose

Kotsu helps students prepare for an exam in time. A student picks the exam board their school follows and the exam they are sitting, and Kotsu spreads that exam's syllabus across the days remaining.

## Who it is for

Students aged 13 and up. Every screen must be understandable by a 13-year-old without help.

At launch, Kotsu serves students in the UK at Key Stage 4 and above: GCSE (KS4) and post-16 qualifications such as A-levels (KS5). Kotsu launches in the UK only, then expands to other countries.

## What Kotsu does

1. The student signs in.
2. The student chooses their exam board.
3. The student chooses the exam they are preparing for.
4. The student enter how many hours he/she can spent per day for 7 days
5. Kotsu produces a study schedule covering the whole syllabus before the exam date, with recap time built in.
6. If the student misses a day or two, for example through illness, Kotsu asks whether to reschedule. If they say yes, everything left is rescheduled. If they say no, the schedule stays as it is.
7. If there is too little time before the exam to fit recap into the schedule, Kotsu reminds the student to recap in their own time.

> Note: this flow describes one exam per schedule. Real students sit many overlapping exams; how Kotsu handles that is undecided (see open questions).

## Principles

- **Simple first.** One decision per screen, plain words, no jargon. If a 13-year-old needs it explained, it is redesigned.
- **The schedule is the product.** Everything else exists to get the student to a schedule they can trust and follow.
- **Same experience everywhere.** Web, iOS and Android show the same schedule from the same backend.
- **Built to travel.** The UK is first, not the only market. Nothing in the product assumes UK key stages, boards or exam names are the only ones that exist.
- **Collect as little as possible.** Most users are under 18. Kotsu stores only what it needs to sign a student in and keep their schedule.
- **Trustworthy content.** Syllabus content is curated by the team and checked against the exam board's published specification.

## Decisions

| Area | Decision |
| --- | --- |
| Accounts | Required. Every student signs in, so their schedule follows them across web and mobile. |
| Launch market | UK only. Other countries follow after launch. |
| Launch audience | Key Stage 4 and above (GCSE and post-16); if Scotland is in scope, its equivalents (National 5, Highers). |
| Launch scope | Several UK exam boards, each with a small set of KS4 and KS5 exams. Each schedule covers one exam (multi-exam scheduling is undecided – see open questions). |
| Study availability | The student enters how many hours they can study on each of the 7 days of the week. The schedule is built from those hours. |
| Missed days | Kotsu never reschedules on its own. It asks the student, and only reschedules if they say yes. |
| Recap | Recap time is part of the schedule. When time is too short to fit it, Kotsu leaves it out and reminds the student to recap in their own time. |
| Syllabus content | Hand-curated by the team and shipped as seed data. No content-management tool at launch. |
| Platforms | Web, iOS and Android, sharing one backend. |
| Initial load | At least 10 live requests per second, rising to as many as 50 at peak times close to exams. |

## Out of scope at launch

- Countries other than the UK.
- Exams below Key Stage 4 (for example KS2 SATs or the 11+).
- Teaching material, revision notes or practice questions. Kotsu schedules the syllabus; it does not teach it.
- Teacher, parent or school accounts.
- Students adding their own exams or syllabus content.
- A content-management tool for non-developers.

## Open questions

- **Multi-exam scheduling.** The flow above covers one exam, but real students sit many overlapping exams sharing the same weekly hours. Undecided: whether launch supports several exams in one schedule, or one schedule per exam; if single-exam at launch, how the engine and data model stay open to multi-exam later.
- **Which boards and exams launch first.** "Several UK boards" is agreed; the named list is not. Candidates are AQA, Pearson Edexcel, OCR, WJEC/Eduqas and CCEA.
- **Scotland – decide before the domain model (phase 2), not phase 13.** Scotland does not use key stages and has its own board (SQA) and qualifications (National 5, Highers). If the phase-2 model bakes in KS levels as types, it will need rework. Qualification levels must be content, not hard-coded KS types, regardless of the Scotland decision.
- **Privacy for under-18s.** The UK age of digital consent is 13, so students aged 13+ can consent for themselves and no parental consent flow is needed at launch. The UK Children's Code still applies to all users under 18 and must be reviewed before accounts are built. Consent ages differ in other countries (up to 16 in parts of Europe) and need revisiting at expansion.
- **What counts as a missed day.** Kotsu cannot know a student was ill; it can only see that scheduled work was not marked complete. Whether one missed day triggers the question, and what happens after a longer gap, needs deciding.
- **After a "no".** Whether Kotsu asks again the next day, or stays quiet until the student asks to reschedule.
- **How much recap.** What share of study time goes to recap, and how little time before the exam counts as too little to fit it.
- **How a student signs in.** Email and password, a school sign-in, or something simpler.
- **Which countries come next,** and in what order.
- **Load after expansion.** The normal and peak targets cover the UK launch. What they become as other countries are added is not decided.
