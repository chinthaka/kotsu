# Mission

## Purpose

Kotsu helps students prepare for their exams in time. A student picks their country, stage and subjects, selects the syllabus topics they want to cover, and Kotsu spreads the selected topics across a short scheduling window, sharing study time across subjects.

## Who it is for

Students aged 13 and up. Every screen must be understandable by a 13-year-old without help.

At launch, Kotsu serves students in England, Wales and Scotland: Key Stages 4–5 in England, National 5 to Advanced Higher in Scotland, Key Stage 4 (GCSE) and Post-16 (AS and A level) in Wales. Kotsu launches in Great Britain only, then expands to other countries.

## What Kotsu does

1. The student chooses how long to schedule for (up to 4 weeks) and their country: England, Wales or Scotland.
2. The student chooses their stage: KS4 or KS5 in England; National 5, Higher (S5) or Advanced Higher (S6) in Scotland; Key Stage 4 (GCSE) or Post-16 (AS and A level) in Wales.
3. The student chooses their subjects for that stage.
4. In England, the student chooses their exam board. In Scotland (SQA) and Wales (WJEC) the board is set automatically.
5. The student selects what to study for each subject: the whole syllabus or specific sub-topics.
6. The student enters how many hours they can study on each of the 7 days of the week.
7. Kotsu produces one study schedule covering the selected topics within the chosen window, sharing the weekly hours across subjects, with recap time built in.
8. If the student misses a day or two, for example through illness, Kotsu asks whether to reschedule. If they say yes, everything left is rescheduled. If they say no, the schedule stays as it is.
9. If there is too little time in the window to fit recap into the schedule, Kotsu reminds the student to recap in their own time.

## Principles

- **Simple first.** One decision per screen, plain words, no jargon. If a 13-year-old needs it explained, it is redesigned.
- **The schedule is the product.** Everything else exists to get the student to one schedule covering all their exams, which they can trust and follow.
- **Same experience everywhere.** Web, iOS and Android work the same way, built on the same backend. Each device keeps its own schedule.
- **Built to travel.** Great Britain is first, not the only market. Nothing in the product assumes British key stages, boards or exam names are the only ones that exist.
- **Collect as little as possible.** Most users are under 18. Kotsu has no accounts and doesn't ask for the student's name. Everything the student enters about their exams, and their schedule and progress, is stored only on their device. Kotsu's servers use it to work out a schedule and keep none of it.
- **Trustworthy content.** Syllabus content is curated by the team and checked against the exam board's published specification.

## Decisions

| Area | Decision |
| --- | --- |
| Accounts | None. There is no sign-in. The schedule and progress are kept on the student's device, and the servers store nothing about the student. A schedule doesn't move between devices, and is lost if the device's storage is cleared, which costs little because a schedule lasts at most 4 weeks. |
| Student name | Not asked for. Kotsu works without knowing who the student is. |
| Exam details | Stored only on the student's device: the schedule window, country, stage, subjects, exam board, chosen topics, weekly study hours, the schedule and progress. The device sends them to the server when a schedule is worked out or changed; the server returns the result and keeps none of them. |
| Launch market | Great Britain only (England, Wales, Scotland). Northern Ireland and other countries follow after launch. |
| Launch audience | England: Key Stage 4 and 5. Scotland: National 5, Higher and Advanced Higher. Wales: Key Stage 4 (GCSE) and Post-16 (AS and A level). |
| Launch scope | England, Wales and Scotland. Each stage launches with a small set of subjects. |
| Subjects per schedule | Many, within one country, stage and board. One schedule covers all the student's subjects, sharing the weekly study hours across them. The hours are split evenly at first; a priority rule may replace this. |
| Topic size | Every sub-topic carries an estimated study time in hours, curated with the content. Topics get time in proportion to it. |
| Topic selection | The student selects what to schedule for each subject: the whole syllabus or specific sub-topics. Kotsu schedules only the selection. |
| Exam boards | The student chooses a board only in England. Scotland (SQA) and Wales (WJEC) have one board each, applied automatically. |
| Schedule window | The student chooses 1 to 4 weeks. The schedule fills that window. |
| Study availability | The student enters how many hours they can study on each of the 7 days of the week. The schedule is built from those hours. |
| Missed days | Kotsu never reschedules on its own. It asks the student, and only reschedules if they say yes. |
| Recap | Recap time is part of the schedule. When time is too short to fit it, Kotsu leaves it out and reminds the student to recap in their own time. |
| Syllabus content | Hand-curated by the team and shipped as seed data. No content-management tool at launch. |
| Platforms | Web, iOS and Android, sharing one backend. |
| Initial load | At least 10 live requests per second, rising to as many as 50 at peak times close to exams. |

## Out of scope at launch

- Countries outside Great Britain.
- Northern Ireland (and its CCEA board) at launch.
- Stages below the launch range (for example KS2 SATs or the 11+ in England).
- Schedule windows longer than 4 weeks.
- Teaching material, revision notes or practice questions. Kotsu schedules the syllabus; it does not teach it.
- Accounts and sign-in of any kind, including teacher, parent or school accounts.
- Moving a schedule from one device to another.
- Students adding their own exams or syllabus content.
- A content-management tool for non-developers.

## Open questions

- **Which boards and subjects launch first.** England's board list is not decided (candidates: AQA, Pearson Edexcel, OCR); Scotland ships SQA and Wales ships WJEC. The subject list per stage is not decided either.
- **Privacy for under-18s.** Kotsu stores nothing about students on its servers, but the UK Children's Code still applies to all users under 18: the app is likely to be used by children, and request logs and device storage are still covered. It must be reviewed before the web client is built. Consent ages differ in other countries (up to 16 in parts of Europe) and need revisiting at expansion.
- **What counts as a missed day.** Kotsu cannot know a student was ill; it can only see that scheduled work was not marked complete. Whether one missed day triggers the question, and what happens after a longer gap, needs deciding.
- **After a "no".** Whether Kotsu asks again the next day, or stays quiet until the student asks to reschedule.
- **Splitting shared hours across subjects.** Kotsu starts with an even split. Whether a priority rule replaces it (for example hardest subject first, or student-chosen priority) is not decided.
- **How much recap.** What share of study time goes to recap, and how little window counts as too little to fit it.
- **Which countries come next,** and in what order.
- **Load after expansion.** The normal and peak targets cover the Great Britain launch. What they become as other countries are added is not decided.
