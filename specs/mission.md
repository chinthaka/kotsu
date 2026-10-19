# Mission

## Purpose

Kotsu helps students prepare for their exams in time. A student picks their country, stage and subjects, selects the syllabus topics they want to cover, and Kotsu spreads the selected topics across a short scheduling window, sharing study time across subjects.

## Who it is for

Students aged 13 and up. Every screen must be understandable by a 13-year-old without help.

At launch, Kotsu serves students in England, Wales and Scotland: Key Stages 4–5 in England, National 5 to Advanced Higher in Scotland, Key Stage 4 (GCSE) and Post-16 (AS and A level) in Wales. Kotsu launches in Great Britain only, then expands to other countries.

## What Kotsu does

1. The student signs in.
2. The student enters their first name (used only to greet them), how long to schedule for (up to 4 weeks), and their country: England, Wales or Scotland.
3. The student chooses their stage: KS4 or KS5 in England; National 5, Higher (S5) or Advanced Higher (S6) in Scotland; Key Stage 4 (GCSE) or Post-16 (AS and A level) in Wales.
4. The student chooses their subjects for that stage.
5. In England, the student chooses their exam board. In Scotland (SQA) and Wales (WJEC) the board is set automatically.
6. The student selects what to study for each subject: the whole syllabus or specific sub-topics.
7. The student enters how many hours they can study on each of the 7 days of the week.
8. Kotsu produces one study schedule covering the selected topics within the chosen window, sharing the weekly hours across subjects, with recap time built in.
9. If the student misses a day or two, for example through illness, Kotsu asks whether to reschedule. If they say yes, everything left is rescheduled. If they say no, the schedule stays as it is.
10. If there is too little time in the window to fit recap into the schedule, Kotsu reminds the student to recap in their own time.

## Principles

- **Simple first.** One decision per screen, plain words, no jargon. If a 13-year-old needs it explained, it is redesigned.
- **The schedule is the product.** Everything else exists to get the student to one schedule covering all their exams, which they can trust and follow.
- **Same experience everywhere.** Web, iOS and Android show the same schedule from the same backend.
- **Built to travel.** Great Britain is first, not the only market. Nothing in the product assumes British key stages, boards or exam names are the only ones that exist.
- **Collect as little as possible.** Most users are under 18. Kotsu stores only what it needs to sign a student in and keep their schedule, plus a first name used only to greet them.
- **Trustworthy content.** Syllabus content is curated by the team and checked against the exam board's published specification.

## Decisions

| Area | Decision |
| --- | --- |
| Accounts | Required. Every student signs in, so their schedule follows them across web and mobile. |
| Student name | A single first name, used only to greet the student. It is not used for anything else. |
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
- Teacher, parent or school accounts.
- Students adding their own exams or syllabus content.
- A content-management tool for non-developers.

## Open questions

- **Which boards and subjects launch first.** England's board list is not decided (candidates: AQA, Pearson Edexcel, OCR); Scotland ships SQA and Wales ships WJEC. The subject list per stage is not decided either.
- **Privacy for under-18s.** The UK age of digital consent is 13, so students aged 13+ can consent for themselves and no parental consent flow is needed at launch. The UK Children's Code still applies to all users under 18 and must be reviewed before accounts are built. Consent ages differ in other countries (up to 16 in parts of Europe) and need revisiting at expansion.
- **What counts as a missed day.** Kotsu cannot know a student was ill; it can only see that scheduled work was not marked complete. Whether one missed day triggers the question, and what happens after a longer gap, needs deciding.
- **After a "no".** Whether Kotsu asks again the next day, or stays quiet until the student asks to reschedule.
- **Splitting shared hours across subjects.** Kotsu starts with an even split. Whether a priority rule replaces it (for example hardest subject first, or student-chosen priority) is not decided.
- **How much recap.** What share of study time goes to recap, and how little window counts as too little to fit it.
- **How a student signs in.** Email and password, a school sign-in, or something simpler.
- **Which countries come next,** and in what order.
- **Load after expansion.** The normal and peak targets cover the Great Britain launch. What they become as other countries are added is not decided.
