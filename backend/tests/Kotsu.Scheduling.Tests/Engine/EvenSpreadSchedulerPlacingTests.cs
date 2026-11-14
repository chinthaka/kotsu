using Kotsu.Scheduling.Domain;
using Kotsu.Scheduling.Engine;

namespace Kotsu.Scheduling.Tests.Engine;

/// <summary>
/// How sessions are placed on dates (D8, D9) and read back from the schedule (D10).
/// </summary>
public class EvenSpreadSchedulerPlacingTests
{
    private static readonly SubjectId _maths = TestContent.Subject("maths");
    private static readonly SubjectId _english = TestContent.Subject("english");

    private static SubjectSelection Whole(Syllabus syllabus) => SubjectSelection.Create(syllabus).Value;

    private static StudySession Session(int dayOfNovember, SubjectId subject, int subTopicNumber, decimal hours) =>
        new(new DateOnly(2026, 11, dayOfNovember), subject, TestContent.Id(subject.Code, subTopicNumber), TestContent.Hours(hours));

    /// <summary>
    /// The worked example in requirement.md: Maths (5, 5 hours) and English (4, 3, 3 hours) in the example week.
    /// </summary>
    private static Schedule WorkedExample() =>
        EvenSpreadScheduler.Build(
            TestCalendars.ExampleCalendar(),
            [Whole(TestContent.Build("maths", 5m, 5m)), Whole(TestContent.Build("english", 4m, 3m, 3m))]).Value;

    [Fact]
    public void Build_WorkedExample_GivesExactlyTheSessionsInTheTable()
    {
        // Act
        var schedule = WorkedExample();

        // Assert
        Assert.Equal(
            [
                Session(2, _maths, 1, 1m),
                Session(2, _english, 1, 1m),
                Session(3, _maths, 1, 1m),
                Session(3, _english, 1, 1m),
                Session(4, _maths, 1, 1m),
                Session(4, _english, 1, 0.5m),
                Session(4, _english, 2, 0.5m),
                Session(5, _maths, 1, 0.5m),
                Session(5, _maths, 2, 0.5m),
                Session(5, _english, 2, 1m),
                Session(6, _maths, 2, 1m),
                Session(6, _english, 2, 0.5m),
                Session(7, _maths, 2, 2m),
                Session(7, _english, 3, 2m),
            ],
            schedule.Sessions);
    }

    [Fact]
    public void Build_WhenEverySubjectNeedsLessThanItsShare_PlacesAllWorkAsEarlyAsPossible()
    {
        // Arrange
        var maths = Whole(TestContent.Build("maths", 3m));
        var english = Whole(TestContent.Build("english", 4m));
        var thursday = new DateOnly(2026, 11, 5);

        // Act
        var schedule = EvenSpreadScheduler.Build(TestCalendars.ExampleCalendar(), [maths, english]).Value;

        // Assert
        Assert.All(schedule.Sessions, s => Assert.True(s.Date <= thursday));
        Assert.Equal(1m, schedule.HoursOn(thursday).Value);
        Assert.Equal(StudyHours.Zero, schedule.HoursOn(new DateOnly(2026, 11, 6)));
        Assert.Equal(StudyHours.Zero, schedule.HoursOn(new DateOnly(2026, 11, 7)));
    }

    [Fact]
    public void Build_ForAWindowStartingMidWeek_PlacesTheFirstSessionsOnTheStartDate()
    {
        // Arrange
        var wednesday = new DateOnly(2026, 11, 4);
        var calendar = TestCalendars.Calendar(wednesday, 1, TestCalendars.ExampleWeek());
        var maths = Whole(TestContent.Build("maths", 5m, 5m));

        // Act
        var schedule = EvenSpreadScheduler.Build(calendar, [maths]).Value;

        // Assert
        Assert.Equal(wednesday, schedule.Sessions[0].Date);
        Assert.Equal(wednesday, schedule.Window.StartDate);
    }

    [Fact]
    public void HoursOn_ADateWithSessions_IsTheSumOfThatDatesSessions()
    {
        // Arrange
        var schedule = WorkedExample();

        // Act
        var hours = schedule.HoursOn(new DateOnly(2026, 11, 4));

        // Assert
        Assert.Equal(2m, hours.Value);
    }

    [Fact]
    public void HoursOn_ADateWithNoSessions_IsZero()
    {
        // Arrange
        var schedule = WorkedExample();

        // Act
        var sunday = schedule.HoursOn(new DateOnly(2026, 11, 8));
        var outsideTheWindow = schedule.HoursOn(new DateOnly(2026, 11, 9));

        // Assert
        Assert.Equal(StudyHours.Zero, sunday);
        Assert.Equal(StudyHours.Zero, outsideTheWindow);
    }

    [Fact]
    public void HoursFor_ASubjectNotInTheSchedule_IsZero()
    {
        // Arrange
        var schedule = WorkedExample();

        // Act
        var hours = schedule.HoursFor(TestContent.Subject("history"));

        // Assert
        Assert.Equal(StudyHours.Zero, hours);
    }
}
