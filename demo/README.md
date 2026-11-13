# Kotsu stakeholder demo

A clickable prototype of the whole student journey, from opening the app to following and changing a study plan. It is plain HTML, CSS and JavaScript with no build step and no server.

## Run it

Open `demo/index.html` in any modern browser (double-click it). That's all.

## What's on screen

- **Left: the presenter guide.** It lists all 13 steps of the journey, with talking points for the current step. Click any step to jump to it. Missing answers are filled in for you.
- **Right: the app**, shown in a phone frame. Every button works.
- **Pretend today is.** Moves the demo's "today" forward or back a day, so you can show what happens when a student misses days.
- **Fill in sample answers** goes straight to a finished plan (England, Key Stage 4, Maths on AQA, Biology on Pearson Edexcel and Chemistry on OCR, 3 weeks). **Reset demo** clears everything and goes back to the welcome screen.

## Suggested walkthrough (about 5 minutes)

1. **Welcome**: no sign-up, no name, data stays on the device.
2. **How long to plan for**: 1 to 4 weeks, with a start date.
3. **Country**: England, Wales or Scotland.
4. **Stage**: changes with the country. Try Scotland to see National 5, Higher and Advanced Higher.
5. **Subjects**: pick several. One plan covers them all.
6. **Exam boards**: choose a board for each subject in England; Scotland (SQA) and Wales (WJEC) are set automatically for every subject.
7. **Topics**: the whole syllabus, or switch it off and pick sub-topics.
8. **Study hours**: hours for each day. The bar shows at once whether the topics fit.
9. **Your schedule**: day by day, with subjects mixed through each day and purple recap sessions at the end of each week.
10. **Mark topics complete**: tap a session. Overall and per-subject progress update.
11. **Missed days**: Kotsu asks "move it forward?". Yes reschedules what's left from today. No keeps the plan, and Kotsu asks again only if more days are missed.
12. **Tight on time**: recap is left out and the student is reminded to recap in their own time.
13. **Your data stays here**: what is saved on the device, what the servers see (the internet address, for a moment, never recorded), and a "Start again" button that clears the plan for a fresh start.

## Rules the demo uses

The real scheduling engine is still being built (see [the roadmap](../specs/roadmap.md)), so the demo uses simple rules to make a believable plan:

- Each day's study time is split **randomly** across the subjects.
- A sub-topic takes as long as its estimated study time, and a long one can be split across days ("part 1 of 2").
- About **15%** of the time is kept for recap, at the end of each week.
- If the time available is less than **110%** of the time the topics need, recap is left out and the student gets the reminder.
- A student is **behind** when a past day still has unticked topics.

## Files

| File | What it holds |
| --- | --- |
| `index.html` | The page: presenter guide and phone frame |
| `styles.css` | The look and feel |
| `data.js` | Sample countries, stages, boards, subjects and topics |
| `app.js` | The screens, the demo's plan maker and the presenter shortcuts |

Like the real app, the demo keeps everything in the browser's own storage (`localStorage`). Unlike the real app, the demo makes its plans in the browser, so nothing is sent anywhere.
