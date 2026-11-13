/*
 * Kotsu stakeholder demo. Static and self-contained: no server, no build step.
 * Everything the "student" enters is kept in this browser's localStorage only,
 * which mirrors Kotsu's promise that a student's data stays on their device.
 *
 * The schedule maker below is a stand-in for the real engine (roadmap phases 4-12).
 * Placeholder rules used by the demo:
 *   - each day's hours are split randomly across subjects
 *   - a sub-topic takes as long as its estimated hours
 *   - about 15% of the time is kept for recap
 *   - recap is left out (with a reminder) when time is under 110% of the topic hours
 *   - "behind" means a past day still has unfinished topics
 */
(() => {
  'use strict';

  const DATA = window.KOTSU_DATA;
  const STORE_KEY = 'kotsu-demo-v1';
  const RECAP_SHARE = 0.15;
  const TIGHT_RATIO = 1.1;
  const MAX_DAY_HOURS = 24;
  const PALETTE = ['#6366f1', '#f59e0b', '#10b981', '#ec4899', '#0ea5e9', '#8b5cf6'];
  const WEEK = [
    ['mon', 'Monday'], ['tue', 'Tuesday'], ['wed', 'Wednesday'], ['thu', 'Thursday'],
    ['fri', 'Friday'], ['sat', 'Saturday'], ['sun', 'Sunday'],
  ];
  const JS_DAY = ['sun', 'mon', 'tue', 'wed', 'thu', 'fri', 'sat'];
  const SETUP = ['window', 'country', 'stage', 'subjects', 'board', 'topics', 'hours'];

  // ---------------------------------------------------------------- dates & numbers
  const pad = (n) => String(n).padStart(2, '0');
  const iso = (d) => `${d.getFullYear()}-${pad(d.getMonth() + 1)}-${pad(d.getDate())}`;
  const parse = (s) => { const [y, m, d] = s.split('-').map(Number); return new Date(y, m - 1, d, 12); };
  const addDays = (s, n) => { const d = parse(s); d.setDate(d.getDate() + n); return iso(d); };
  const weekdayKey = (s) => JS_DAY[parse(s).getDay()];
  const fmtShort = (s) => parse(s).toLocaleDateString('en-GB', { weekday: 'short', day: 'numeric', month: 'short' });
  const fmtLong = (s) => parse(s).toLocaleDateString('en-GB', { weekday: 'long', day: 'numeric', month: 'long' });
  const fmtNoDay = (s) => parse(s).toLocaleDateString('en-GB', { day: 'numeric', month: 'long' });
  const sum = (xs, f = (x) => x) => xs.reduce((t, x) => t + f(x), 0);
  const floorHalf = (x) => Math.floor(x * 2 + 1e-9) / 2;
  const ceilHalf = (x) => Math.ceil(x * 2 - 1e-9) / 2;
  const hrs = (h) => {
    if (h === 0) return '0 hrs';
    const whole = Math.floor(h);
    const label = (whole ? whole : '') + (h % 1 ? '½' : '');
    return `${label} ${h <= 1 ? 'hr' : 'hrs'}`;
  };
  const plural = (n, word) => `${n} ${word}${n === 1 ? '' : 's'}`;

  // A small seeded random generator, so a schedule doesn't change on every redraw.
  const seeded = (seed) => () => {
    seed = (seed + 0x6d2b79f5) | 0;
    let t = Math.imul(seed ^ (seed >>> 15), 1 | seed);
    t = (t + Math.imul(t ^ (t >>> 7), 61 | t)) ^ t;
    return ((t ^ (t >>> 14)) >>> 0) / 4294967296;
  };

  // ---------------------------------------------------------------- content lookups
  const country = () => DATA.countries.find((c) => c.id === state.country);
  const stage = () => country()?.stages.find((s) => s.id === state.stage);
  const board = (sid) => country()?.boards.find((b) => b.id === state.boards[sid]);
  const subject = (id) => DATA.subjects[id];
  const colourOf = (id) => PALETTE[Math.max(0, state.subjects.indexOf(id)) % PALETTE.length];
  const subIds = (sid) => subject(sid).topics.flatMap((t, ti) => t.subs.map((_, si) => `${sid}:${ti}:${si}`));
  const subInfo = (key) => {
    const [sid, ti, si] = key.split(':');
    const topic = subject(sid).topics[ti];
    const [name, hours] = topic.subs[si];
    return { key, subject: sid, topic: topic.name, name, hours };
  };
  const selectedItems = () => state.subjects.flatMap((sid) => (state.selection[sid] || []).map(subInfo));
  const neededHours = () => sum(selectedItems(), (i) => i.hours);
  const weekHours = () => sum(WEEK, ([k]) => state.hours[k]);
  const windowDates = () => Array.from({ length: state.weeks * 7 }, (_, i) => addDays(state.start, i));
  const windowEnd = () => addDays(state.start, state.weeks * 7 - 1);

  // ---------------------------------------------------------------- state
  function freshState() {
    const today = iso(new Date());
    return {
      screen: 'welcome',
      weeks: 2,
      start: today,
      country: null,
      stage: null,
      subjects: [],
      boards: {},
      selection: {},
      whole: {},
      open: {},
      hours: { mon: 1.5, tue: 1.5, wed: 1.5, thu: 1.5, fri: 1, sat: 3, sun: 2 },
      schedule: null,
      today,
      dismissedUpTo: null,
      seed: 7,
      lastStory: null,
    };
  }

  function load() {
    try {
      const saved = JSON.parse(localStorage.getItem(STORE_KEY));
      if (saved && saved.screen) {
        const { board: legacyBoard, ...rest } = saved;
        const boards = saved.boards || Object.fromEntries(
          (saved.subjects || []).map((sid) => [sid, legacyBoard]).filter(([, id]) => id),
        );
        return { ...freshState(), ...rest, boards, confirmStartAgain: false };
      }
    } catch { /* fall through to a fresh start */ }
    return freshState();
  }

  let state = load();
  let toastTimer = null;
  let buildTimer = null;
  const save = () => localStorage.setItem(STORE_KEY, JSON.stringify(state));

  // ---------------------------------------------------------------- the stand-in schedule maker
  /**
   * days:  [{ date, free }] free = hours still open on that date
   * items: [{ key, subject, topic, name, hours }] work still to place
   */
  function plan(days, items, seed) {
    const rand = seeded(seed);
    const free = sum(days, (d) => d.free);
    const needed = sum(items, (i) => i.hours);
    const tight = needed > 0 && free < needed * TIGHT_RATIO;
    const recapBudget = tight ? 0 : Math.max(0, Math.min(floorHalf(free * RECAP_SHARE), floorHalf(free - needed)));

    const out = days.map((d) => ({ date: d.date, recap: 0, topicCap: d.free, sessions: [] }));

    // Reserve recap at the end of each week, a share in proportion to that week's hours.
    const chunks = [];
    for (let i = 0; i < out.length; i += 7) chunks.push(out.slice(i, i + 7));
    if (recapBudget > 0) {
      let left = recapBudget;
      chunks.forEach((chunk, ci) => {
        const chunkFree = sum(chunk, (d) => d.topicCap);
        let share = ci === chunks.length - 1 ? left : Math.min(left, ceilHalf((recapBudget * chunkFree) / free));
        for (let d = chunk.length - 1; d >= 0 && share > 0; d--) {
          const take = Math.min(share, floorHalf(chunk[d].topicCap / 2), 1.5);
          if (take <= 0) continue;
          chunk[d].recap += take;
          chunk[d].topicCap -= take;
          share -= take;
          left -= take;
        }
      });
    }

    // Spread the topics across the whole window rather than front-loading them.
    const topicFree = sum(out, (d) => d.topicCap);
    const spread = topicFree > 0 ? Math.min(1, needed / topicFree) : 0;

    const queues = {};
    items.forEach((i) => (queues[i.subject] ||= []).push({ ...i, left: i.hours }));
    const remaining = (s) => sum(queues[s], (q) => q.left);
    let n = 0;

    for (const day of out) {
      const slots = Math.min(day.topicCap, ceilHalf(day.topicCap * spread)) * 2;
      const pending = Object.fromEntries(Object.keys(queues).map((s) => [s, remaining(s)]));
      const alloc = {};
      for (let i = 0; i < slots; i++) {
        const open = Object.keys(pending).filter((s) => pending[s] > 0);
        if (!open.length) break;
        const pick = open[Math.floor(rand() * open.length)]; // the random split across subjects
        alloc[pick] = (alloc[pick] || 0) + 0.5;
        pending[pick] -= 0.5;
      }
      const order = Object.keys(alloc).sort(() => rand() - 0.5);
      for (const s of order) {
        let a = alloc[s];
        while (a > 0 && queues[s].length) {
          const item = queues[s][0];
          const take = Math.min(a, item.left);
          day.sessions.push({ id: `s${seed}-${n++}`, kind: 'topic', subject: s, key: item.key, hours: take, done: false });
          item.left -= take;
          a -= take;
          if (item.left <= 0) queues[s].shift();
        }
      }
    }

    // Label each recap session with the subjects studied that week.
    chunks.forEach((chunk) => {
      const studied = [...new Set(chunk.flatMap((d) => d.sessions.map((s) => s.subject)))];
      chunk.forEach((d) => {
        if (d.recap > 0) {
          d.sessions.push({ id: `s${seed}-${n++}`, kind: 'recap', subjects: studied.length ? studied : [...state.subjects], hours: d.recap, done: false });
        }
      });
    });

    const unscheduled = Object.values(queues).flat().filter((q) => q.left > 0).map((q) => q.key);
    return { days: out.map((d) => ({ date: d.date, sessions: d.sessions })), tight, unscheduled, recapHours: recapBudget };
  }

  function buildSchedule() {
    const days = windowDates().map((date) => ({ date, free: state.hours[weekdayKey(date)] }));
    const result = plan(days, selectedItems(), state.seed);
    state.schedule = { ...result, days: result.days.map((d, i) => ({ ...d, hours: days[i].free })), madeOn: state.start };
    state.today = state.start;
    state.dismissedUpTo = null;
  }

  /** "Yes" to the reschedule question: keep what's done, move everything else from today. */
  function reschedule() {
    const sch = state.schedule;
    const doneHours = {};
    sch.days.forEach((d) => d.sessions.forEach((s) => {
      if (s.kind === 'topic' && s.done) doneHours[s.key] = (doneHours[s.key] || 0) + s.hours;
    }));

    const future = [];
    sch.days.forEach((d) => {
      const unfinished = d.sessions.some((s) => s.kind === 'topic' && !s.done);
      if (d.date < state.today) {
        if (unfinished) d.moved = true;
        d.sessions = d.sessions.filter((s) => s.done);
      } else {
        d.sessions = d.sessions.filter((s) => s.done);
        future.push({ date: d.date, free: Math.max(0, d.hours - sum(d.sessions, (s) => s.hours)) });
      }
    });

    const items = selectedItems()
      .map((i) => ({ ...i, hours: ceilHalf(i.hours - (doneHours[i.key] || 0)) }))
      .filter((i) => i.hours > 0);

    state.seed += 1;
    const result = plan(future, items, state.seed);
    result.days.forEach((rd) => {
      const day = sch.days.find((d) => d.date === rd.date);
      day.sessions = [...day.sessions, ...rd.sessions];
    });
    sch.tight = result.tight;
    sch.unscheduled = result.unscheduled;
    sch.recapHours = result.recapHours;
    sch.madeOn = state.today;
    state.dismissedUpTo = null;
  }

  function missedDays() {
    const sch = state.schedule;
    if (!sch) return [];
    return sch.days.filter((d) => d.date < state.today
      && (!state.dismissedUpTo || d.date > state.dismissedUpTo)
      && d.sessions.some((s) => s.kind === 'topic' && !s.done));
  }

  function progress(filter = () => true) {
    const topics = (state.schedule?.days || []).flatMap((d) => d.sessions).filter((s) => s.kind === 'topic' && filter(s));
    const total = sum(topics, (s) => s.hours);
    const done = sum(topics.filter((s) => s.done), (s) => s.hours);
    return total ? Math.round((done / total) * 100) : 0;
  }

  /** How many parts each sub-topic was split into, for "part 1 of 2" labels. */
  function partLabels() {
    const seen = {};
    const labels = {};
    const counts = {};
    (state.schedule?.days || []).forEach((d) => d.sessions.forEach((s) => {
      if (s.kind === 'topic') counts[s.key] = (counts[s.key] || 0) + 1;
    }));
    (state.schedule?.days || []).forEach((d) => d.sessions.forEach((s) => {
      if (s.kind !== 'topic' || counts[s.key] < 2) return;
      seen[s.key] = (seen[s.key] || 0) + 1;
      labels[s.id] = `part ${seen[s.key]} of ${counts[s.key]}`;
    }));
    return labels;
  }

  // ---------------------------------------------------------------- the presenter guide
  const JOURNEY = [
    { id: 'welcome', title: 'Welcome', notes: [
      'No sign-up, no account and no name. A student can start straight away.',
      'The privacy promise is the first thing they see: everything stays on this device.',
      'Built for students aged 13 and up: plain words, one decision per screen.'] },
    { id: 'window', title: 'How long to plan for', notes: [
      'The student picks a window of 1 to 4 weeks.',
      'Every schedule fills that window, from the start date to the end date.'] },
    { id: 'country', title: 'Country', notes: [
      'Launch is in Great Britain: England, Wales and Scotland.',
      'Countries are data, not code, so new countries can be added later without a redesign.'] },
    { id: 'stage', title: 'Stage', notes: [
      'Each country has its own stages: Key Stage 4 and 5 in England, National 5 to Advanced Higher in Scotland, GCSE and Post-16 in Wales.',
      'Try switching country to show how the stages change.'] },
    { id: 'subjects', title: 'Subjects', notes: [
      'The student picks as many subjects as they like.',
      'One schedule covers all of them, sharing the study time across subjects.'] },
    { id: 'board', title: 'Exam boards', notes: [
      'In England, each subject can use a different board (for example, Maths on AQA and English on Pearson Edexcel).',
      'In Scotland (SQA) and Wales (WJEC) there is one board, so it is set automatically for every subject.'] },
    { id: 'topics', title: 'Topics', notes: [
      'For each subject: the whole syllabus, or just the sub-topics they want.',
      'Every sub-topic has an estimated study time, so Kotsu knows how much work there is.'] },
    { id: 'hours', title: 'Study hours', notes: [
      'The student enters how long they can study on each day of the week, in half-hour steps.',
      'The bar at the bottom shows straight away whether the topics will fit.'] },
    { id: 'schedule', title: 'Your schedule', story: 'schedule', notes: [
      'One schedule, day by day, covering every subject.',
      'Each day’s time is split randomly across the subjects, so they mix through the week.',
      'Recap sessions (purple) are built in at the end of each week.'] },
    { id: 'complete', title: 'Mark topics complete', story: 'complete', notes: [
      'Tap a session to mark it done. Progress updates at the top.',
      'Progress is kept on this device only.'] },
    { id: 'missed', title: 'Missed days', story: 'missed', notes: [
      'We pretend two days have passed with nothing ticked off.',
      'Kotsu never reschedules on its own. It asks first.',
      '“Yes” moves everything left to start from today. “No” keeps the plan as it is.'] },
    { id: 'tight', title: 'Tight on time', story: 'tight', notes: [
      'Here the window is only just long enough for the topics.',
      'Recap is left out, and the student is reminded to recap in their own time.'] },
    { id: 'privacy', title: 'Your data stays here', notes: [
      'Shows exactly what is saved, and that it is saved only on this device.',
      'Explains in plain words what the servers see: the internet address, for a moment, never recorded.',
      '“Start again” clears the plan so the student can begin fresh. Uninstalling the app removes everything too.',
      'Backing up the device is the student’s own choice; Kotsu doesn’t block it.'] },
  ];

  function currentJourneyId() {
    if (['schedule', 'building'].includes(state.screen)) return state.lastStory || 'schedule';
    return state.screen;
  }

  function renderPresenter() {
    const current = currentJourneyId();
    const idx = JOURNEY.findIndex((j) => j.id === current);
    const step = JOURNEY[Math.max(0, idx)];
    const onSchedule = state.screen === 'schedule' && state.schedule;
    document.getElementById('presenter').innerHTML = `
      <h2>The student journey</h2>
      <ol class="journey">
        ${JOURNEY.map((j, i) => `
          <li class="${j.id === current ? 'active' : i < idx ? 'done' : ''}">
            <button data-action="jump" data-arg="${j.id}"><span class="num">${i < idx ? '✓' : i + 1}</span>${j.title}</button>
          </li>`).join('')}
      </ol>
      <div class="notes">
        <h3>${step.title}</h3>
        <ul>${step.notes.map((n) => `<li>${n}</li>`).join('')}</ul>
      </div>
      <div class="controls">
        <div class="control-row">
          <span class="control-label">Pretend today is</span>
          <button class="pill-btn" data-action="today" data-arg="-1" ${onSchedule ? '' : 'disabled'}>−1 day</button>
          <span class="today-chip">${fmtShort(state.today)}</span>
          <button class="pill-btn" data-action="today" data-arg="1" ${onSchedule ? '' : 'disabled'}>+1 day</button>
        </div>
        <div class="control-row">
          <span class="control-label">Shortcuts</span>
          <button class="pill-btn primary" data-action="sample">Fill in sample answers</button>
          <button class="pill-btn danger" data-action="reset">Reset demo</button>
        </div>
      </div>`;
  }

  // ---------------------------------------------------------------- screens
  function head(stepId) {
    const i = SETUP.indexOf(stepId);
    return `
      <div class="app-head"><div class="row">
        <button class="back" data-action="back" aria-label="Back">‹</button>
        <div class="steps">${SETUP.map((_, k) => `<span class="${k <= i ? 'on' : ''}"></span>`).join('')}</div>
        <span class="step-count">${i + 1} of ${SETUP.length}</span>
      </div></div>`;
  }

  function foot(label, enabled, hint = '') {
    return `<div class="app-foot">
      <button class="cta" data-action="next" ${enabled ? '' : 'disabled'}>${label}</button>
      <div class="foot-hint">${hint}</div></div>`;
  }

  const tick = (on) => `<span class="tick">${on ? '✓' : ''}</span>`;

  const screens = {
    welcome: () => `
      <div class="welcome">
        <div class="logo-xl">K</div>
        <h1>Your exams,<br>planned for you.</h1>
        <p class="lead">Tell Kotsu what you're studying and when you're free. You'll get one simple plan for every subject.</p>
        <div class="how">
          <div><span>1</span> Pick your subjects and topics</div>
          <div><span>2</span> Say how long you can study each day</div>
          <div><span>3</span> Follow your day-by-day plan</div>
        </div>
        <div class="privacy-card"><span class="i">🔒</span>
          <div><b>No sign-up. No name needed.</b><br>Everything you enter stays on this device.</div></div>
        <button class="cta" data-action="next" style="margin-top:18px">Let's start</button>
      </div>`,

    window: () => {
      const starts = [
        ['Today', iso(new Date())],
        ['Tomorrow', addDays(iso(new Date()), 1)],
        ['Next Monday', (() => { let d = addDays(iso(new Date()), 1); while (weekdayKey(d) !== 'mon') d = addDays(d, 1); return d; })()],
      ];
      return `${head('window')}
      <div class="app-body">
        <h1 class="q">How long do you want to plan for?</h1>
        <p class="sub">Pick how many weeks your plan should cover.</p>
        <div class="options grid2">
          ${[1, 2, 3, 4].map((w) => `
            <button class="opt ${state.weeks === w ? 'on' : ''}" data-action="weeks" data-arg="${w}">
              <span class="big">${w}</span><div><div class="t">${w === 1 ? 'week' : 'weeks'}</div><div class="h">${w * 7} days</div></div>
            </button>`).join('')}
        </div>
        <div class="section-label">When do you want to start?</div>
        <div class="chips">
          ${starts.map(([label, d]) => `<button class="chip ${state.start === d ? 'on' : ''}" data-action="start" data-arg="${d}">${label}</button>`).join('')}
        </div>
        <div class="info"><span class="i">📅</span><div>Your plan runs from <b>${fmtLong(state.start)}</b> to <b>${fmtLong(windowEnd())}</b>.</div></div>
      </div>${foot('Next', true)}`;
    },

    country: () => `${head('country')}
      <div class="app-body">
        <h1 class="q">Where do you go to school?</h1>
        <p class="sub">This tells Kotsu which exams you're taking.</p>
        <div class="options">
          ${DATA.countries.map((c) => `
            <button class="opt ${state.country === c.id ? 'on' : ''}" data-action="country" data-arg="${c.id}">
              <span class="flag" style="background:${c.colour}">${c.badge}</span>
              <div><div class="t">${c.name}</div><div class="h">${c.stages.map((s) => s.name).join(' · ')}</div></div>
              ${tick(state.country === c.id)}
            </button>`).join('')}
        </div>
        <div class="info"><span class="i">🌍</span><div>More countries are coming soon.</div></div>
      </div>${foot('Next', !!state.country)}`,

    stage: () => `${head('stage')}
      <div class="app-body">
        <h1 class="q">Which stage are you at?</h1>
        <p class="sub">Pick the exams you're getting ready for in ${country().name}.</p>
        <div class="options">
          ${country().stages.map((s) => `
            <button class="opt ${state.stage === s.id ? 'on' : ''}" data-action="stage" data-arg="${s.id}">
              <div><div class="t">${s.name}</div><div class="h">${s.hint}</div></div>
              ${tick(state.stage === s.id)}
            </button>`).join('')}
        </div>
      </div>${foot('Next', !!state.stage)}`,

    subjects: () => `${head('subjects')}
      <div class="app-body">
        <h1 class="q">Which subjects are you studying?</h1>
        <p class="sub">Pick as many as you like. You'll get one plan for all of them.</p>
        <div class="options grid2">
          ${stage().subjects.map((id) => {
            const on = state.subjects.includes(id);
            const s = subject(id);
            return `<button class="opt tile ${on ? 'on' : ''}" data-action="subject" data-arg="${id}">
              <span class="emoji">${s.icon}</span><div class="t">${s.name}</div>
              <div class="h">${plural(s.topics.length, 'topic')}</div>${tick(on)}</button>`;
          }).join('')}
        </div>
      </div>${foot(state.subjects.length ? `Next · ${plural(state.subjects.length, 'subject')}` : 'Next', state.subjects.length > 0, state.subjects.length ? '' : 'Pick at least one subject')}`,

    board: () => {
      const c = country();
      return `${head('board')}
      <div class="app-body">
        <h1 class="q">Which exam boards do your subjects use?</h1>
        <p class="sub">Each subject has one board. Not sure? Ask your teacher.</p>
        ${state.subjects.map((sid) => `
          <div class="subject-card">
            <div class="subject-head"><div class="t">${subject(sid).name}</div></div>
            ${c.boards.length === 1
              ? `<div class="opt on" style="cursor:default">
                  <div><div class="t">${c.boards[0].name}</div><div class="h">Set automatically for this subject</div></div>${tick(true)}
                </div>`
              : `<div class="options">
                  ${c.boards.map((b) => `
                    <button class="opt ${state.boards[sid] === b.id ? 'on' : ''}" data-action="board" data-arg="${sid}:${b.id}">
                      <div><div class="t">${b.name}</div></div>${tick(state.boards[sid] === b.id)}
                    </button>`).join('')}
                </div>`}
          </div>`).join('')}
        ${c.boards.length === 1
          ? `<div class="info"><span class="i">✨</span><div>${c.name === 'Scotland' ? 'Scottish' : 'Welsh'} subjects all use <b>${c.boards[0].name}</b>, so there is nothing to choose.</div></div>`
          : `<div class="info"><span class="i">💡</span><div>Different subjects can use different boards.</div></div>`}
      </div>${foot('Next', state.subjects.every((sid) => state.boards[sid]))}`;
    },

    topics: () => {
      const missing = state.subjects.filter((sid) => !(state.selection[sid] || []).length);
      return `${head('topics')}
      <div class="app-body">
        <h1 class="q">What do you want to study?</h1>
        <p class="sub">Keep the whole syllabus, or turn it off to pick just the parts you need.</p>
        ${state.subjects.map((sid) => {
          const s = subject(sid);
          const sel = state.selection[sid] || [];
          const whole = state.whole[sid];
          const h = sum(sel.map(subInfo), (i) => i.hours);
          const colour = colourOf(sid);
          return `<div class="subject-card">
            <div class="subject-head">
              <span class="dot" style="background:${colour}22">${s.icon}</span>
              <div><div class="t">${s.name}</div><div class="h">${plural(sel.length, 'part')} · about ${hrs(h)}</div></div>
              <button class="switch ${whole ? 'on' : ''}" data-action="whole" data-arg="${sid}">Whole syllabus <i></i></button>
            </div>
            ${whole ? '' : `<div class="topic-list">
              ${s.topics.map((t, ti) => `<div class="topic-name">${t.name}</div>
                ${t.subs.map(([name, hh], si) => {
                  const key = `${sid}:${ti}:${si}`;
                  const on = sel.includes(key);
                  return `<button class="sub-row ${on ? 'on' : ''}" data-action="sub" data-arg="${key}">
                    <span class="box">${on ? '✓' : ''}</span>${name}<span class="hrs">${hrs(hh)}</span></button>`;
                }).join('')}`).join('')}
            </div>`}
          </div>`;
        }).join('')}
      </div>${foot('Next', missing.length === 0, missing.length ? `Pick at least one part of ${missing.map((m) => subject(m).name).join(', ')}` : `About ${hrs(neededHours())} of study in total`)}`;
    },

    hours: () => {
      const total = weekHours() * state.weeks;
      const needed = neededHours();
      const ratio = needed ? total / needed : 0;
      const fit = total === 0 ? null
        : ratio < 1 ? ['bad', '😬 Not everything will fit', `Your topics need about <b>${hrs(needed)}</b>, but you have <b>${hrs(total)}</b>. Add some hours or pick fewer topics.`]
        : ratio < TIGHT_RATIO ? ['warn', '⏳ It fits, just', `Your topics need about <b>${hrs(needed)}</b> and you have <b>${hrs(total)}</b>. There won't be room for recap.`]
        : ['good', '👍 That fits', `Your topics need about <b>${hrs(needed)}</b> and you have <b>${hrs(total)}</b>, with time left over for recap.`];
      return `${head('hours')}
      <div class="app-body">
        <h1 class="q">How long can you study each day?</h1>
        <p class="sub">Think about a normal week. Use 0 for days off.</p>
        ${WEEK.map(([k, name]) => `
          <div class="day-row">
            <span class="d">${name}</span>
            <span class="bar"><i style="width:${(state.hours[k] / 6) * 100}%"></i></span>
            <span class="stepper">
              <button data-action="hours" data-arg="${k}:-0.5" aria-label="Less on ${name}">−</button>
              <span>${hrs(state.hours[k])}</span>
              <button data-action="hours" data-arg="${k}:0.5" aria-label="More on ${name}">+</button>
            </span>
          </div>`).join('')}
        ${fit ? `<div class="fit ${fit[0]}"><b>${fit[1]}</b>
          <div class="meter"><i style="width:${Math.min(100, ratio * 100 / 1.3)}%"></i></div>${fit[2]}</div>` : ''}
      </div>${foot('Make my plan', total > 0, total > 0 ? `${hrs(weekHours())} a week · ${hrs(total)} in total` : 'Add some hours to at least one day')}`;
    },

    building: () => `
      <div class="building"><div>
        <div class="spinner"></div>
        <h1 class="q">Making your plan…</h1>
        <ul>
          <li>✓ Sharing your time across ${plural(state.subjects.length, 'subject')}</li>
          <li>✓ Fitting in ${plural(selectedItems().length, 'topic')}</li>
          <li>✓ Adding recap time</li>
          <li>✓ Saving it on this device</li>
        </ul>
      </div></div>`,

    schedule: renderSchedule,
    privacy: renderPrivacy,
  };

  function renderSchedule() {
    const sch = state.schedule;
    const labels = partLabels();
    const pct = progress();
    const start = sch.days[0].date;
    const end = sch.days[sch.days.length - 1].date;
    const missed = missedDays();
    const weeks = [];
    for (let i = 0; i < sch.days.length; i += 7) weeks.push(sch.days.slice(i, i + 7));

    const sessionHtml = (s) => {
      if (s.kind === 'recap') {
        return `<button class="session recap ${s.done ? 'done' : ''}" data-action="toggle" data-arg="${s.id}">
          <span class="stripe" style="background:var(--recap)"></span>
          <div><div class="t">🔁 Recap: ${s.subjects.map((x) => subject(x).name).join(', ')}</div>
          <div class="h">${hrs(s.hours)} · go back over this week's topics</div></div>
          <span class="check">${s.done ? '✓' : ''}</span></button>`;
      }
      const info = subInfo(s.key);
      return `<button class="session ${s.done ? 'done' : ''}" data-action="toggle" data-arg="${s.id}">
        <span class="stripe" style="background:${colourOf(s.subject)}"></span>
        <div><div class="t">${info.name}</div>
        <div class="h">${subject(s.subject).name} · ${info.topic} · ${hrs(s.hours)}${labels[s.id] ? ` · ${labels[s.id]}` : ''}</div></div>
        <span class="check">${s.done ? '✓' : ''}</span></button>`;
    };

    const dayHtml = (d) => {
      const isToday = d.date === state.today;
      const past = d.date < state.today;
      const unfinished = d.sessions.some((s) => s.kind === 'topic' && !s.done);
      const allDone = d.sessions.length > 0 && d.sessions.every((s) => s.done);
      const badge = isToday ? '<span class="badge today">Today</span>'
        : d.moved ? '<span class="badge moved">Moved forward</span>'
        : past && unfinished ? '<span class="badge missed">Missed</span>'
        : allDone ? '<span class="badge alldone">Done</span>'
        : !d.sessions.length ? '<span class="badge free">Free</span>' : '';
      return `<div class="day ${isToday ? 'today' : ''} ${past ? 'past' : ''} ${past && unfinished ? 'missed' : ''}" ${isToday ? 'id="today"' : ''}>
        <div class="day-head"><div><b>${fmtShort(d.date)}</b> <span class="h">· ${hrs(sum(d.sessions, (s) => s.hours))}</span></div>${badge}</div>
        ${d.sessions.length ? d.sessions.map(sessionHtml).join('')
          : `<div class="day-empty">${d.moved ? 'This work has moved to later days.' : d.hours ? 'Nothing planned. Enjoy the break!' : 'Day off.'}</div>`}
      </div>`;
    };

    const status = state.today < start ? `Starts ${fmtShort(start)}`
      : state.today > end ? 'Your plan has finished'
      : `Day ${parse(state.today).getTime() / 864e5 - parse(start).getTime() / 864e5 + 1} of ${sch.days.length}`;

    return `
      <div class="sched-head">
        <div class="row">
          <div>
            <div class="meta">${status}</div>
            <h1>My plan</h1>
            <div class="meta">${fmtNoDay(start)} – ${fmtNoDay(end)} · ${stage().name}</div>
            <div class="meta">${state.subjects.map((sid) => `${subject(sid).name}: ${board(sid).name}`).join(' · ')}</div>
          </div>
          <div class="ring" style="--p:${pct}"><span>${pct}%</span></div>
        </div>
        <div class="subj-progress">
          ${state.subjects.map((sid) => `<div><i style="background:${colourOf(sid)}"></i>${subject(sid).name} ${progress((s) => s.subject === sid)}%</div>`).join('')}
        </div>
        <div class="row" style="margin-top:12px; justify-content:flex-start; gap:8px">
          <button class="icon-btn" data-action="goto" data-arg="hours">✏️ Change plan</button>
          <button class="icon-btn" data-action="goto" data-arg="privacy">🔒 My data</button>
        </div>
      </div>
      <div class="app-body" id="sched-body">
        ${sch.tight ? `<div class="banner recap"><span class="i">🔁</span><div><b>Remember to recap in your own time.</b><br>Your plan is tight, so there's no room for recap sessions. Even 10 minutes at the end of a day helps.</div></div>` : ''}
        ${sch.unscheduled.length ? `<div class="banner warn"><span class="i">⚠️</span><div><b>${plural(sch.unscheduled.length, 'topic')} didn't fit.</b><br>Add more study hours, or pick fewer topics.<br><button data-action="goto" data-arg="hours">Change my hours</button></div></div>` : ''}
        ${!sch.tight && !sch.unscheduled.length && sch.recapHours > 0 ? `<div class="banner good"><span class="i">✅</span><div>Everything fits, with <b>${hrs(sch.recapHours)}</b> of recap built in.</div></div>` : ''}
        ${weeks.map((w, i) => `<div class="week-label">Week ${i + 1}</div>${w.map(dayHtml).join('')}`).join('')}
      </div>
      ${missed.length ? `
        <div class="modal-wrap"><div class="modal">
          <div class="emoji">🤒</div>
          <h2>Looks like you missed ${missed.length === 1 ? 'a day' : `${missed.length} days`}</h2>
          <p>That's OK, it happens. Do you want Kotsu to move what's left so it starts from today?</p>
          <div class="missed-list">${missed.map((d) => `<span>${fmtShort(d.date)}</span>`).join('')}</div>
          <div class="btns">
            <button class="cta" data-action="resched-yes">Yes, move it forward</button>
            <button class="cta ghost" data-action="resched-no">No, keep my plan</button>
          </div>
        </div></div>` : ''}`;
  }

  function renderPrivacy() {
    const items = selectedItems();
    return `
      <div class="app-head"><div class="row">
        <button class="back" data-action="goto" data-arg="schedule" aria-label="Back">‹</button>
        <span class="step-count">My data</span><span style="width:36px"></span>
      </div></div>
      <div class="app-body">
        <h1 class="q">Your data stays on this device</h1>
        <p class="sub">This is everything Kotsu knows about your plan. It's saved here, and nowhere else.</p>
        <div class="store-list">
          <div><span>Plan length</span><b>${plural(state.weeks, 'week')}</b></div>
          <div><span>Country</span><b>${country().name}</b></div>
          <div><span>Stage</span><b>${stage().name}</b></div>
          <div><span>Exam boards by subject</span><b>${state.subjects.map((sid) => `${subject(sid).name}: ${board(sid).name}`).join(', ')}</b></div>
          <div><span>Subjects</span><b>${state.subjects.map((s) => subject(s).name).join(', ')}</b></div>
          <div><span>Topics chosen</span><b>${items.length}</b></div>
          <div><span>Study hours</span><b>${hrs(weekHours())} a week</b></div>
          <div><span>Your plan and progress</span><b>${progress()}% done</b></div>
        </div>
        <div class="never">
          <div><i>✓</i>No account and no sign-in</div>
          <div><i>✓</i>We never ask for your name</div>
          <div><i>✓</i>Kotsu's servers save none of this</div>
          <div><i>✓</i>No ads, no tracking, no analytics</div>
        </div>
        <div class="section-label">What Kotsu's servers see</div>
        <div class="info" style="margin-top:0"><span class="i">🌐</span><div>To make your plan, this device sends your choices to Kotsu, which sends the plan back and <b>saves none of it</b>. Like any website, Kotsu sees your device's internet address while it connects. It's used for a moment to keep the service safe, and <b>never recorded</b>.</div></div>
        <div class="info"><span class="i">📱</span><div>Your plan doesn't move to other phones or computers. If you back up your device, your plan is in your backup. That's up to you.</div></div>
        <button class="cta ghost" data-action="start-again" style="margin-top:18px">Start again</button>
        <div class="foot-hint">Clears your plan so you can make a new one.</div>
      </div>
      ${state.confirmStartAgain ? `
        <div class="modal-wrap"><div class="modal">
          <div class="emoji">🔄</div>
          <h2>Start again?</h2>
          <p>This clears your plan and your progress from this device, so you can make a new plan from the beginning.</p>
          <div class="btns">
            <button class="cta" data-action="start-again-yes">Yes, start again</button>
            <button class="cta ghost" data-action="start-again-no">No, keep my plan</button>
          </div>
        </div></div>` : ''}`;
  }

  // ---------------------------------------------------------------- rendering
  function render() {
    if (state.screen === 'schedule' && !state.schedule) state.screen = 'hours';
    const scr = document.getElementById('screen');
    const prevBody = scr.querySelector('.app-body');
    const keepScroll = prevBody && scr.dataset.screen === state.screen ? prevBody.scrollTop : null;
    scr.innerHTML = screens[state.screen]();
    scr.dataset.screen = state.screen;
    const body = scr.querySelector('.app-body');
    if (body && keepScroll !== null) body.scrollTop = keepScroll;
    renderPresenter();
    save();
  }

  function scrollToToday() {
    const t = document.getElementById('today');
    const body = document.getElementById('sched-body');
    if (t && body) body.scrollTop = t.offsetTop - body.offsetTop - 10;
  }

  function toast(msg) {
    const el = document.getElementById('toast');
    el.textContent = msg;
    el.hidden = false;
    clearTimeout(toastTimer);
    toastTimer = setTimeout(() => { el.hidden = true; }, 2600);
  }

  function go(screen) {
    clearTimeout(buildTimer);
    state.screen = screen;
    render();
    const body = document.querySelector('.app-body');
    if (body) body.scrollTop = 0;
    if (screen === 'schedule') scrollToToday();
  }

  function makePlan() {
    state.lastStory = 'schedule';
    go('building');
    buildTimer = setTimeout(() => {
      buildSchedule();
      go('schedule');
    }, 1900);
  }

  const ORDER = ['welcome', ...SETUP];
  function next() {
    const i = ORDER.indexOf(state.screen);
    if (state.screen === 'hours') return makePlan();
    go(ORDER[i + 1]);
  }
  function back() {
    const i = ORDER.indexOf(state.screen);
    go(ORDER[Math.max(0, i - 1)]);
  }

  // ---------------------------------------------------------------- sample answers and stories
  function fillSample() {
    state.weeks = 3;
    state.start = iso(new Date());
    state.country = 'england';
    state.stage = 'england-ks4';
    state.subjects = ['maths-gcse', 'biology-gcse', 'chemistry'];
    state.boards = { 'maths-gcse': 'aqa', 'biology-gcse': 'pearson-edexcel', chemistry: 'ocr' };
    state.subjects.forEach((sid) => { state.whole[sid] = true; state.selection[sid] = subIds(sid); });
    state.hours = { mon: 2, tue: 2, wed: 2, thu: 2, fri: 1.5, sat: 3.5, sun: 3 };
  }

  const ready = () => state.country && state.stage && state.subjects.length
    && state.subjects.every((sid) => state.boards[sid])
    && state.subjects.every((s) => (state.selection[s] || []).length) && weekHours() > 0;

  function showStory(id) {
    clearTimeout(buildTimer);
    if (!ready()) fillSample();
    state.lastStory = id;

    if (id === 'tight') {
      // Make the window only just long enough: total hours = topic hours, rounded up.
      state.weeks = 1;
      let slots = Math.ceil(neededHours() * 2);
      const perDay = Math.floor(slots / 7);
      WEEK.forEach(([k], i) => { state.hours[k] = Math.min(MAX_DAY_HOURS, (perDay + (i >= 7 - (slots % 7) ? 1 : 0)) / 2); });
      buildSchedule();
    } else if (id === 'missed') {
      if (!state.schedule || state.schedule.tight) buildSchedule();
      state.dismissedUpTo = null;
      state.today = addDays(state.schedule.days[0].date, 2);
      state.schedule.days.filter((d) => d.date < state.today).forEach((d) => d.sessions.forEach((s) => { s.done = false; }));
    } else if (id === 'complete') {
      if (!state.schedule || state.schedule.tight) buildSchedule();
      state.today = state.schedule.days[0].date;
      const first = state.schedule.days.find((d) => d.sessions.length);
      if (first) first.sessions[0].done = true;
      toast('Tap any session to tick it off ✓');
    } else if (!state.schedule) {
      buildSchedule();
    }
    go('schedule');
  }

  function jump(id) {
    const step = JOURNEY.find((j) => j.id === id);
    if (step.story) return showStory(step.story);
    if (id === 'privacy') {
      if (!ready()) fillSample();
      if (!state.schedule) buildSchedule();
      state.lastStory = null;
      return go('privacy');
    }
    // Setup screens need their earlier answers; fill them in when jumping ahead.
    const needs = { stage: ['country'], subjects: ['country', 'stage'], board: ['country', 'stage', 'subjects'], topics: ['country', 'stage', 'subjects'] };
    if ((needs[id] || []).some((k) => !state[k] || (Array.isArray(state[k]) && !state[k].length))) fillSample();
    if (id === 'topics') {
      state.subjects.forEach((sid) => { state.boards[sid] ||= country().boards[0].id; });
    }
    go(id);
  }

  // ---------------------------------------------------------------- actions
  const actions = {
    next,
    back,
    jump,
    goto: (s) => { if (s === 'privacy') state.lastStory = null; state.confirmStartAgain = false; go(s); },
    weeks: (w) => { state.weeks = Number(w); render(); },
    start: (d) => { state.start = d; render(); },
    country: (id) => {
      if (state.country !== id) {
        Object.assign(state, { country: id, stage: null, subjects: [], selection: {}, whole: {}, boards: {} });
      }
      render();
    },
    stage: (id) => {
      if (state.stage !== id) Object.assign(state, { stage: id, subjects: [], selection: {}, whole: {}, boards: {} });
      render();
    },
    subject: (id) => {
      if (state.subjects.includes(id)) {
        state.subjects = state.subjects.filter((s) => s !== id);
        delete state.selection[id];
        delete state.boards[id];
      } else {
        state.subjects.push(id);
        state.whole[id] = true;
        state.selection[id] = subIds(id);
        if (country().boards.length === 1) state.boards[id] = country().boards[0].id;
      }
      render();
    },
    board: (arg) => {
      const [sid, id] = arg.split(':');
      state.boards[sid] = id;
      render();
    },
    whole: (sid) => {
      state.whole[sid] = !state.whole[sid];
      if (state.whole[sid]) state.selection[sid] = subIds(sid);
      render();
    },
    sub: (key) => {
      const sid = key.split(':')[0];
      const sel = state.selection[sid] || [];
      state.selection[sid] = sel.includes(key) ? sel.filter((k) => k !== key) : subIds(sid).filter((k) => sel.includes(k) || k === key);
      render();
    },
    hours: (arg) => {
      const [k, delta] = arg.split(':');
      state.hours[k] = Math.max(0, Math.min(MAX_DAY_HOURS, state.hours[k] + Number(delta)));
      render();
    },
    toggle: (id) => {
      const s = state.schedule.days.flatMap((d) => d.sessions).find((x) => x.id === id);
      s.done = !s.done;
      if (s.done) toast(progress() === 100 ? '🎉 Everything done. Brilliant work!' : 'Nice one! Ticked off ✓');
      render();
    },
    'resched-yes': () => {
      reschedule();
      render();
      scrollToToday();
      toast('Done! Everything left now starts from today.');
    },
    'resched-no': () => {
      const missed = missedDays();
      state.dismissedUpTo = missed[missed.length - 1].date;
      render();
      toast('OK, your plan stays as it is.');
    },
    today: (n) => {
      state.today = addDays(state.today, Number(n));
      if (state.today < state.dismissedUpTo) state.dismissedUpTo = null;
      render();
      scrollToToday();
    },
    sample: () => { fillSample(); state.lastStory = null; makePlan(); },
    reset: () => { clearTimeout(buildTimer); state = freshState(); go('welcome'); },
    'start-again': () => { state.confirmStartAgain = true; render(); },
    'start-again-no': () => { state.confirmStartAgain = false; render(); },
    'start-again-yes': () => { clearTimeout(buildTimer); localStorage.removeItem(STORE_KEY); state = freshState(); go('welcome'); toast('Your plan was cleared. Ready for a fresh start.'); },
  };

  document.addEventListener('click', (e) => {
    const el = e.target.closest('[data-action]');
    if (!el || el.disabled) return;
    actions[el.dataset.action]?.(el.dataset.arg);
  });

  const tickClock = () => {
    const d = new Date();
    document.getElementById('clock').textContent = `${pad(d.getHours())}:${pad(d.getMinutes())}`;
  };
  tickClock();
  setInterval(tickClock, 30000);

  if (state.screen === 'building') state.screen = state.schedule ? 'schedule' : 'hours';
  render();
  if (state.screen === 'schedule') scrollToToday();
})();
