using Kotsu.Scheduling.Domain;

namespace Kotsu.Scheduling.Tests;

/// <summary>
/// Builds made-up syllabus content for tests. Sub-topics are numbered across the whole
/// syllabus, so a subject "maths" with hours 5, 5 has sub-topics "maths-1" and "maths-2".
/// Nothing here names a real country, stage, board or subject.
/// </summary>
internal static class TestContent
{
    public const string DefaultStage = "stage-a";
    public const string DefaultBoard = "board-a";

    public static StudyHours Hours(decimal hours) => StudyHours.Create(hours).Value;

    public static SubjectId Subject(string code) => SubjectId.Create(code).Value;

    public static SubTopicId Id(string subject, int number) => SubTopicId.Create($"{subject}-{number}").Value;

    /// <summary>
    /// One topic holding one sub-topic per amount of hours, on the default stage and board.
    /// </summary>
    public static Syllabus Build(string subject, params decimal[] subTopicHours) =>
        Build(subject, DefaultStage, DefaultBoard, subTopicHours);

    /// <summary>
    /// One topic holding one sub-topic per amount of hours, on the given stage and board.
    /// </summary>
    public static Syllabus Build(string subject, string stage, string board, params decimal[] subTopicHours) =>
        BuildWithTopics(subject, stage, board, subTopicHours);

    /// <summary>
    /// One topic per array, each holding one sub-topic per amount of hours.
    /// </summary>
    public static Syllabus BuildWithTopics(string subject, string stage, string board, params decimal[][] topics)
    {
        var number = 0;
        var built = topics.Select((hours, index) =>
            Topic.Create(
                TopicId.Create($"{subject}-topic-{index + 1}").Value,
                $"{subject} topic {index + 1}",
                hours.Select(h =>
                {
                    number++;
                    return SubTopic.Create(Id(subject, number), $"{subject} {number}", Hours(h)).Value;
                }).ToList()).Value).ToList();

        return Syllabus.Create(
            StageId.Create(stage).Value,
            Subject(subject),
            BoardId.Create(board).Value,
            built).Value;
    }
}
