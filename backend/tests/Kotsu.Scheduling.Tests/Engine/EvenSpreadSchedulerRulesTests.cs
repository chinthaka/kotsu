using Kotsu.Scheduling.Domain;
using Kotsu.Scheduling.Engine;

namespace Kotsu.Scheduling.Tests.Engine;

public class EvenSpreadSchedulerRulesTests
{
    private static SubjectSelection Whole(Syllabus syllabus) => SubjectSelection.Create(syllabus).Value;

    [Fact]
    public void Build_WithNoSelections_FailsWithNoSubjects()
    {
        // Act
        var result = EvenSpreadScheduler.Build(TestCalendars.ExampleCalendar(), []);

        // Assert
        Assert.Equal(["schedule.no-subjects"], result.Errors.Select(e => e.Code));
    }

    [Fact]
    public void Build_WithTheSameSubjectTwice_FailsWithDuplicateSubject()
    {
        // Arrange
        var maths = Whole(TestContent.Build("maths", 1m));
        var mathsAgain = Whole(TestContent.Build("maths", 2m));

        // Act
        var result = EvenSpreadScheduler.Build(TestCalendars.ExampleCalendar(), [maths, mathsAgain]);

        // Assert
        Assert.Equal(["schedule.duplicate-subject"], result.Errors.Select(e => e.Code));
    }

    [Fact]
    public void Build_WithSyllabiFromDifferentStages_FailsWithMixedStage()
    {
        // Arrange
        var maths = Whole(TestContent.Build("maths", "stage-a", "board-a", 1m));
        var english = Whole(TestContent.Build("english", "stage-b", "board-a", 1m));

        // Act
        var result = EvenSpreadScheduler.Build(TestCalendars.ExampleCalendar(), [maths, english]);

        // Assert
        Assert.Equal(["schedule.mixed-stage"], result.Errors.Select(e => e.Code));
    }

    [Fact]
    public void Build_WithDifferentSubjectsOnDifferentBoards_Succeeds()
    {
        // Arrange
        var maths = Whole(TestContent.Build("maths", "stage-a", "board-a", 1m));
        var english = Whole(TestContent.Build("english", "stage-a", "board-b", 1m));

        // Act
        var result = EvenSpreadScheduler.Build(TestCalendars.ExampleCalendar(), [maths, english]);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public void Build_WithADuplicateSubjectAndAMixedStage_ReportsBoth()
    {
        // Arrange
        var maths = Whole(TestContent.Build("maths", "stage-a", "board-a", 1m));
        var mathsAgain = Whole(TestContent.Build("maths", "stage-b", "board-a", 1m));

        // Act
        var result = EvenSpreadScheduler.Build(TestCalendars.ExampleCalendar(), [maths, mathsAgain]);

        // Assert
        Assert.Equal(
            ["schedule.duplicate-subject", "schedule.mixed-stage"],
            result.Errors.Select(e => e.Code).Order());
    }

    [Fact]
    public void Build_WithMoreSubTopicsThanHalfHours_FailsWithNotEnoughHours()
    {
        // Arrange
        var oneHourWeek = TestCalendars.Calendar(TestCalendars.Monday, 1, TestCalendars.Week(1m, 0m, 0m, 0m, 0m, 0m, 0m));
        var maths = Whole(TestContent.Build("maths", 1m, 1m, 1m));

        // Act
        var result = EvenSpreadScheduler.Build(oneHourWeek, [maths]);

        // Assert
        Assert.Equal(["schedule.not-enough-hours"], result.Errors.Select(e => e.Code));
    }

    [Fact]
    public void Build_WithNoHoursAtAll_FailsWithNotEnoughHoursAndDoesNotThrow()
    {
        // Arrange
        var noHours = TestCalendars.Calendar(TestCalendars.Monday, 1, TestCalendars.EveryDay(0m));
        var maths = Whole(TestContent.Build("maths", 1m));

        // Act
        var result = EvenSpreadScheduler.Build(noHours, [maths]);

        // Assert
        Assert.Equal(["schedule.not-enough-hours"], result.Errors.Select(e => e.Code));
    }
}
