using Kotsu.Scheduling.Domain;
using Kotsu.Scheduling.Engine;

namespace Kotsu.Scheduling.Tests.Engine;

/// <summary>
/// The roadmap's "Done when" as checks that must hold for every schedule, run over a range of inputs.
/// </summary>
public class ScheduleInvariantsTests
{
    private sealed record Case(StudyCalendar Calendar, IReadOnlyList<SubjectSelection> Selections);

    private static SubjectSelection Whole(Syllabus syllabus) => SubjectSelection.Create(syllabus).Value;

    private static readonly Dictionary<string, Func<Case>> _cases = new()
    {
        ["worked example"] = () => new(
            TestCalendars.ExampleCalendar(),
            [Whole(TestContent.Build("maths", 5m, 5m)), Whole(TestContent.Build("english", 4m, 3m, 3m))]),

        ["one subject needs less than its share"] = () => new(
            TestCalendars.ExampleCalendar(),
            [Whole(TestContent.Build("maths", 2m)), Whole(TestContent.Build("english", 20m))]),

        ["three subjects, one needs less than its share"] = () => new(
            TestCalendars.ExampleCalendar(),
            [Whole(TestContent.Build("a", 1m)), Whole(TestContent.Build("b", 20m)), Whole(TestContent.Build("c", 20m))]),

        ["every subject needs less than its share"] = () => new(
            TestCalendars.ExampleCalendar(),
            [Whole(TestContent.Build("maths", 3m)), Whole(TestContent.Build("english", 4m))]),

        ["lower limit of a half-hour per sub-topic"] = () => new(
            TestCalendars.Calendar(TestCalendars.Monday, 1, TestCalendars.Week(1m, 1m, 1m, 1m, 1m, 1m, 0m)),
            [
                Whole(TestContent.Build("a", 0.5m, 0.5m, 0.5m, 0.5m, 0.5m, 0.5m, 0.5m, 0.5m, 0.5m, 0.5m)),
                Whole(TestContent.Build("b", 20m)),
            ]),

        ["four weeks, three subjects"] = () => new(
            TestCalendars.Calendar(TestCalendars.Monday, 4, TestCalendars.ExampleWeek()),
            [
                Whole(TestContent.Build("maths", 5m, 5m, 6m)),
                Whole(TestContent.Build("english", 4m, 3m, 3m, 2m)),
                Whole(TestContent.Build("history", 10m, 8m)),
            ]),

        ["window starting mid-week"] = () => new(
            TestCalendars.Calendar(new DateOnly(2026, 11, 4), 2, TestCalendars.ExampleWeek()),
            [Whole(TestContent.Build("maths", 5m, 5m)), Whole(TestContent.Build("english", 4m, 3m, 3m))]),

        ["half an hour every day"] = () => new(
            TestCalendars.Calendar(TestCalendars.Monday, 1, TestCalendars.EveryDay(0.5m)),
            [Whole(TestContent.Build("maths", 1m, 1m)), Whole(TestContent.Build("english", 2m))]),

        ["one subject with one sub-topic"] = () => new(
            TestCalendars.ExampleCalendar(),
            [Whole(TestContent.Build("maths", 3m))]),
    };

    public static TheoryData<string> CaseNames => [.. _cases.Keys];

    [Theory]
    [MemberData(nameof(CaseNames))]
    public void Build_ForEveryCase_KeepsTheScheduleInvariants(string caseName)
    {
        // Arrange
        var (calendar, selections) = _cases[caseName]();

        // Act
        var schedule = EvenSpreadScheduler.Build(calendar, selections).Value;

        // Assert
        AssertEverySelectedSubTopicIsScheduledExactlyOnce(schedule, selections);
        AssertEverySessionIsInsideTheWindowOnAStudyDate(schedule, calendar);
        AssertNoDateHasMoreWorkThanItsHours(schedule, calendar);
        AssertTheHoursAddUp(schedule, calendar, selections);
    }

    [Fact]
    public void Build_Twice_GivesTheSameSessions()
    {
        // Arrange
        var (calendar, selections) = _cases["four weeks, three subjects"]();

        // Act
        var first = EvenSpreadScheduler.Build(calendar, selections).Value;
        var second = EvenSpreadScheduler.Build(calendar, selections).Value;

        // Assert
        Assert.Equal(first.Sessions, second.Sessions);
        Assert.Equal(first.UnusedHours, second.UnusedHours);
    }

    private static void AssertEverySelectedSubTopicIsScheduledExactlyOnce(Schedule schedule, IReadOnlyList<SubjectSelection> selections)
    {
        var selected = selections.SelectMany(s => s.SubTopics.Select(t => t.Id)).ToList();
        var scheduled = schedule.Sessions.Select(s => s.SubTopicId).Distinct().ToList();

        // Part 1: every selected sub-topic has a session, and nothing else does.
        Assert.Equal(selected.Select(id => id.Code).Order(), scheduled.Select(id => id.Code).Order());

        // Part 2: within a subject's sessions, each sub-topic's sessions form one unbroken run,
        // so no other sub-topic of the same subject comes between its first and last session.
        foreach (var selection in selections)
        {
            var subjectSessions = schedule.Sessions.Where(s => s.SubjectId == selection.SubjectId).ToList();
            foreach (var subTopic in selection.SubTopics)
            {
                var positions = subjectSessions
                    .Select((s, index) => (s.SubTopicId, index))
                    .Where(p => p.SubTopicId == subTopic.Id)
                    .Select(p => p.index)
                    .ToList();
                Assert.Equal(Enumerable.Range(positions[0], positions.Count), positions);
            }
        }
    }

    private static void AssertEverySessionIsInsideTheWindowOnAStudyDate(Schedule schedule, StudyCalendar calendar)
    {
        var studyDates = calendar.Days.Where(d => d.Hours > StudyHours.Zero).Select(d => d.Date).ToList();

        Assert.All(schedule.Sessions, s =>
        {
            Assert.InRange(s.Date, calendar.Window.StartDate, calendar.Window.EndDate);
            Assert.Contains(s.Date, studyDates);
            Assert.True(s.Hours > StudyHours.Zero);
        });

        // A sub-topic has at most one session per date.
        Assert.All(
            schedule.Sessions.GroupBy(s => (s.SubTopicId, s.Date)),
            group => Assert.Single(group));
    }

    private static void AssertNoDateHasMoreWorkThanItsHours(Schedule schedule, StudyCalendar calendar)
    {
        Assert.All(calendar.Days, day => Assert.True(schedule.HoursOn(day.Date) <= day.Hours, $"{day.Date} is over its hours"));
    }

    private static void AssertTheHoursAddUp(Schedule schedule, StudyCalendar calendar, IReadOnlyList<SubjectSelection> selections)
    {
        foreach (var selection in selections)
        {
            var fromSessions = schedule.Sessions.Where(s => s.SubjectId == selection.SubjectId).Sum(s => s.Hours.Value);
            Assert.Equal(fromSessions, schedule.HoursFor(selection.SubjectId).Value);
        }

        var placed = schedule.Sessions.Sum(s => s.Hours.Value);
        Assert.Equal(calendar.TotalHours.Value, placed + schedule.UnusedHours.Value);
    }
}
