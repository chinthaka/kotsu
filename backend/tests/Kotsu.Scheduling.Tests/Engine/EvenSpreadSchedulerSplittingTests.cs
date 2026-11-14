using Kotsu.Scheduling.Domain;
using Kotsu.Scheduling.Engine;

namespace Kotsu.Scheduling.Tests.Engine;

/// <summary>
/// How a subject's hours are split across its sub-topics (D7). Phase 5 replaces this even split
/// with one proportional to estimated hours.
/// </summary>
public class EvenSpreadSchedulerSplittingTests
{
    private static SubjectSelection Whole(Syllabus syllabus) => SubjectSelection.Create(syllabus).Value;

    private static decimal HoursForSubTopic(Schedule schedule, SubTopicId subTopicId) =>
        schedule.Sessions.Where(s => s.SubTopicId == subTopicId).Sum(s => s.Hours.Value);

    [Fact]
    public void Build_WorkedExample_SplitsEachSubjectsHoursEvenlyWithTheSpareHalfHourToTheFirstSubTopic()
    {
        // Arrange
        var maths = Whole(TestContent.Build("maths", 5m, 5m));
        var english = Whole(TestContent.Build("english", 4m, 3m, 3m));

        // Act
        var schedule = EvenSpreadScheduler.Build(TestCalendars.ExampleCalendar(), [maths, english]).Value;

        // Assert
        Assert.Equal(3.5m, HoursForSubTopic(schedule, TestContent.Id("maths", 1)));
        Assert.Equal(3.5m, HoursForSubTopic(schedule, TestContent.Id("maths", 2)));
        Assert.Equal(2.5m, HoursForSubTopic(schedule, TestContent.Id("english", 1)));
        Assert.Equal(2m, HoursForSubTopic(schedule, TestContent.Id("english", 2)));
        Assert.Equal(2m, HoursForSubTopic(schedule, TestContent.Id("english", 3)));
    }

    [Fact]
    public void Build_WithSubTopicsOfDifferentSizes_StillSplitsEvenly()
    {
        // Arrange: this is phase 4's even spread. Phase 5 changes it to weight by size.
        var maths = Whole(TestContent.Build("maths", 0.5m, 3.5m));

        // Act
        var schedule = EvenSpreadScheduler.Build(TestCalendars.ExampleCalendar(), [maths]).Value;

        // Assert
        Assert.Equal(4m, schedule.HoursFor(maths.SubjectId).Value);
        Assert.Equal(2m, HoursForSubTopic(schedule, TestContent.Id("maths", 1)));
        Assert.Equal(2m, HoursForSubTopic(schedule, TestContent.Id("maths", 2)));
    }
}
