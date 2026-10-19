using Kotsu.Scheduling.Domain;

namespace Kotsu.Scheduling.Tests.Domain;

public class SyllabusTests
{
    private static readonly StageId _stage = StageId.Create("sqa-national-5").Value;
    private static readonly SubjectId _subject = SubjectId.Create("mathematics").Value;
    private static readonly BoardId _board = BoardId.Create("sqa").Value;

    private static StudyHours Hours(decimal hours) => StudyHours.Create(hours).Value;

    private static SubTopic NewSubTopic(string code, decimal hours) =>
        SubTopic.Create(SubTopicId.Create(code).Value, code, Hours(hours)).Value;

    private static Topic NewTopic(string code, params SubTopic[] subTopics) =>
        Topic.Create(TopicId.Create(code).Value, code, subTopics).Value;

    [Fact]
    public void SubTopic_WithHalfAnHour_Succeeds()
    {
        // Act
        var result = SubTopic.Create(SubTopicId.Create("surds").Value, "Surds", Hours(0.5m));

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(0.5m, result.Value.EstimatedHours.Value);
    }

    [Fact]
    public void SubTopic_WithZeroHours_FailsWithTooFewHours()
    {
        // Act
        var result = SubTopic.Create(SubTopicId.Create("surds").Value, "Surds", StudyHours.Zero);

        // Assert
        Assert.Equal(["sub-topic.too-few-hours"], result.Errors.Select(e => e.Code));
    }

    [Fact]
    public void SubTopic_WithBlankName_FailsWithBlankName()
    {
        // Act
        var result = SubTopic.Create(SubTopicId.Create("surds").Value, "", Hours(1m));

        // Assert
        Assert.Equal(["name.blank"], result.Errors.Select(e => e.Code));
    }

    [Fact]
    public void Topic_EstimatedHours_IsTheSumOfItsSubTopics()
    {
        // Arrange
        var topic = NewTopic("expressions", NewSubTopic("surds", 1.5m), NewSubTopic("indices", 2m), NewSubTopic("brackets", 0.5m));

        // Act
        var hours = topic.EstimatedHours;

        // Assert
        Assert.Equal(4m, hours.Value);
    }

    [Fact]
    public void Topic_WithNoSubTopics_FailsWithNoSubTopics()
    {
        // Act
        var result = Topic.Create(TopicId.Create("expressions").Value, "Expressions", []);

        // Assert
        Assert.Equal(["topic.no-sub-topics"], result.Errors.Select(e => e.Code));
    }

    [Fact]
    public void Topic_WithDuplicateSubTopicIds_FailsWithDuplicateSubTopic()
    {
        // Act
        var result = Topic.Create(
            TopicId.Create("expressions").Value, "Expressions", [NewSubTopic("surds", 1m), NewSubTopic("surds", 2m)]);

        // Assert
        Assert.Equal(["topic.duplicate-sub-topic"], result.Errors.Select(e => e.Code));
    }

    [Fact]
    public void Syllabus_WithTopics_IsIdentifiedByStageSubjectAndBoard()
    {
        // Arrange
        Topic[] topics = [NewTopic("expressions", NewSubTopic("surds", 1m)), NewTopic("geometry", NewSubTopic("circles", 2m))];

        // Act
        var result = Syllabus.Create(_stage, _subject, _board, topics);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(_stage, result.Value.StageId);
        Assert.Equal(_subject, result.Value.SubjectId);
        Assert.Equal(_board, result.Value.BoardId);
        Assert.Equal(topics, result.Value.Topics);
    }

    [Fact]
    public void Syllabus_WithNoTopics_FailsWithNoTopics()
    {
        // Act
        var result = Syllabus.Create(_stage, _subject, _board, []);

        // Assert
        Assert.Equal(["syllabus.no-topics"], result.Errors.Select(e => e.Code));
    }

    [Fact]
    public void Syllabus_WithDuplicateTopicIds_FailsWithDuplicateTopic()
    {
        // Act
        var result = Syllabus.Create(_stage, _subject, _board,
            [NewTopic("expressions", NewSubTopic("surds", 1m)), NewTopic("expressions", NewSubTopic("indices", 1m))]);

        // Assert
        Assert.Equal(["syllabus.duplicate-topic"], result.Errors.Select(e => e.Code));
    }

    [Fact]
    public void Syllabus_WithSameSubTopicIdInTwoTopics_FailsWithDuplicateSubTopic()
    {
        // Act
        var result = Syllabus.Create(_stage, _subject, _board,
            [NewTopic("expressions", NewSubTopic("surds", 1m)), NewTopic("number", NewSubTopic("surds", 1m))]);

        // Assert
        Assert.Equal(["syllabus.duplicate-sub-topic"], result.Errors.Select(e => e.Code));
    }
}
