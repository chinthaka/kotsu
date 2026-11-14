using Kotsu.Scheduling.Domain;

namespace Kotsu.Scheduling.Tests.Domain;

public class SubjectSelectionTests
{
    /// <summary>
    /// Two topics: the first with maths-1 and maths-2, the second with maths-3.
    /// </summary>
    private static Syllabus TwoTopicSyllabus() =>
        TestContent.BuildWithTopics("maths", TestContent.DefaultStage, TestContent.DefaultBoard, [1m, 2m], [3m]);

    [Fact]
    public void Create_WholeSyllabus_SelectsEverySubTopicInSyllabusOrder()
    {
        // Arrange
        var syllabus = TwoTopicSyllabus();

        // Act
        var result = SubjectSelection.Create(syllabus);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(
            [TestContent.Id("maths", 1), TestContent.Id("maths", 2), TestContent.Id("maths", 3)],
            result.Value.SubTopics.Select(s => s.Id));
    }

    [Fact]
    public void Create_WholeSyllabus_KnowsItsSubjectAndSyllabus()
    {
        // Arrange
        var syllabus = TwoTopicSyllabus();

        // Act
        var selection = SubjectSelection.Create(syllabus).Value;

        // Assert
        Assert.Equal(TestContent.Subject("maths"), selection.SubjectId);
        Assert.Same(syllabus, selection.Syllabus);
    }

    [Fact]
    public void Create_SpecificSubTopics_SelectsOnlyThoseInSyllabusOrder()
    {
        // Arrange
        var syllabus = TwoTopicSyllabus();
        var reversed = new[] { TestContent.Id("maths", 3), TestContent.Id("maths", 1) };

        // Act
        var result = SubjectSelection.Create(syllabus, reversed);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(
            [TestContent.Id("maths", 1), TestContent.Id("maths", 3)],
            result.Value.SubTopics.Select(s => s.Id));
    }

    [Fact]
    public void Create_WithARepeatedSubTopic_SelectsItOnce()
    {
        // Arrange
        var syllabus = TwoTopicSyllabus();
        var repeated = new[] { TestContent.Id("maths", 2), TestContent.Id("maths", 2) };

        // Act
        var selection = SubjectSelection.Create(syllabus, repeated).Value;

        // Assert
        Assert.Equal([TestContent.Id("maths", 2)], selection.SubTopics.Select(s => s.Id));
    }

    [Fact]
    public void Create_WithNoSubTopics_FailsWithNoSubTopics()
    {
        // Arrange
        var syllabus = TwoTopicSyllabus();

        // Act
        var result = SubjectSelection.Create(syllabus, []);

        // Assert
        Assert.Equal(["subject-selection.no-sub-topics"], result.Errors.Select(e => e.Code));
    }

    [Fact]
    public void Create_WithASubTopicNotInTheSyllabus_FailsWithUnknownSubTopic()
    {
        // Arrange
        var syllabus = TwoTopicSyllabus();
        var unknown = new[] { TestContent.Id("maths", 1), TestContent.Id("english", 1) };

        // Act
        var result = SubjectSelection.Create(syllabus, unknown);

        // Assert
        Assert.Equal(["subject-selection.unknown-sub-topic"], result.Errors.Select(e => e.Code));
    }
}
