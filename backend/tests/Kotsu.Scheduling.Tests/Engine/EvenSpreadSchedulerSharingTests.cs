using Kotsu.Scheduling.Domain;
using Kotsu.Scheduling.Engine;

namespace Kotsu.Scheduling.Tests.Engine;

/// <summary>
/// How the window's hours are shared across subjects (D6). Every test uses the 13.5-hour example week
/// unless it says otherwise.
/// </summary>
public class EvenSpreadSchedulerSharingTests
{
    private static SubjectSelection Whole(Syllabus syllabus) => SubjectSelection.Create(syllabus).Value;

    private static Schedule Build(params SubjectSelection[] selections) =>
        EvenSpreadScheduler.Build(TestCalendars.ExampleCalendar(), selections).Value;

    [Fact]
    public void Build_WhenTwoSubjectsNeedMoreThanTheWindow_SplitsEvenlyWithTheSpareHalfHourToTheFirst()
    {
        // Arrange
        var maths = Whole(TestContent.Build("maths", 5m, 5m));
        var english = Whole(TestContent.Build("english", 4m, 3m, 3m));

        // Act
        var schedule = Build(maths, english);

        // Assert
        Assert.Equal(7m, schedule.HoursFor(maths.SubjectId).Value);
        Assert.Equal(6.5m, schedule.HoursFor(english.SubjectId).Value);
        Assert.Equal(StudyHours.Zero, schedule.UnusedHours);
    }

    [Fact]
    public void Build_WhenTheSubjectsAreGivenInTheOtherOrder_GivesTheSpareHalfHourToTheNewFirst()
    {
        // Arrange
        var maths = Whole(TestContent.Build("maths", 5m, 5m));
        var english = Whole(TestContent.Build("english", 4m, 3m, 3m));

        // Act
        var schedule = Build(english, maths);

        // Assert
        Assert.Equal(7m, schedule.HoursFor(english.SubjectId).Value);
        Assert.Equal(6.5m, schedule.HoursFor(maths.SubjectId).Value);
    }

    [Fact]
    public void Build_WhenOneSubjectNeedsLessThanItsShare_GivesTheRestToTheOther()
    {
        // Arrange
        var maths = Whole(TestContent.Build("maths", 2m));
        var english = Whole(TestContent.Build("english", 20m));

        // Act
        var schedule = Build(maths, english);

        // Assert
        Assert.Equal(2m, schedule.HoursFor(maths.SubjectId).Value);
        Assert.Equal(11.5m, schedule.HoursFor(english.SubjectId).Value);
        Assert.Equal(StudyHours.Zero, schedule.UnusedHours);
    }

    [Fact]
    public void Build_WhenOneSubjectNeedsLessThanItsShare_SharesTheRestEvenlyAcrossTheOthers()
    {
        // Arrange
        var a = Whole(TestContent.Build("a", 1m));
        var b = Whole(TestContent.Build("b", 20m));
        var c = Whole(TestContent.Build("c", 20m));

        // Act
        var schedule = Build(a, b, c);

        // Assert
        Assert.Equal(1m, schedule.HoursFor(a.SubjectId).Value);
        Assert.Equal(6.5m, schedule.HoursFor(b.SubjectId).Value);
        Assert.Equal(6m, schedule.HoursFor(c.SubjectId).Value);
        Assert.Equal(StudyHours.Zero, schedule.UnusedHours);
    }

    [Fact]
    public void Build_WhenEverySubjectNeedsLessThanItsShare_GivesEachWhatItNeedsAndReportsTheRestUnused()
    {
        // Arrange
        var maths = Whole(TestContent.Build("maths", 3m));
        var english = Whole(TestContent.Build("english", 4m));

        // Act
        var schedule = Build(maths, english);

        // Assert
        Assert.Equal(3m, schedule.HoursFor(maths.SubjectId).Value);
        Assert.Equal(4m, schedule.HoursFor(english.SubjectId).Value);
        Assert.Equal(6.5m, schedule.UnusedHours.Value);
    }

    [Fact]
    public void Build_NeverGivesASubjectFewerHalfHoursThanItHasSubTopics()
    {
        // Arrange: 6 hours, or 12 half-hours, in the week.
        var sixHourWeek = TestCalendars.Calendar(TestCalendars.Monday, 1, TestCalendars.Week(1m, 1m, 1m, 1m, 1m, 1m, 0m));
        var a = Whole(TestContent.Build("a", 0.5m, 0.5m, 0.5m, 0.5m, 0.5m, 0.5m, 0.5m, 0.5m, 0.5m, 0.5m));
        var b = Whole(TestContent.Build("b", 20m));

        // Act
        var schedule = EvenSpreadScheduler.Build(sixHourWeek, [a, b]).Value;

        // Assert
        Assert.Equal(5m, schedule.HoursFor(a.SubjectId).Value);
        Assert.Equal(1m, schedule.HoursFor(b.SubjectId).Value);
        Assert.Equal(
            a.SubTopics.Select(s => s.Id.Code).Concat(b.SubTopics.Select(s => s.Id.Code)).Order(),
            schedule.Sessions.Select(s => s.SubTopicId.Code).Distinct().Order());
    }
}
