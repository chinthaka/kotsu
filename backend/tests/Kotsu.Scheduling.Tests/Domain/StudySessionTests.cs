using Kotsu.Scheduling.Domain;

namespace Kotsu.Scheduling.Tests.Domain;

public class StudySessionTests
{
    [Fact]
    public void TwoSessions_WithTheSameDateSubjectSubTopicAndHours_AreEqual()
    {
        // Arrange
        var date = new DateOnly(2026, 11, 2);
        var first = new StudySession(date, TestContent.Subject("maths"), TestContent.Id("maths", 1), TestContent.Hours(1m));
        var second = new StudySession(date, TestContent.Subject("maths"), TestContent.Id("maths", 1), TestContent.Hours(1m));

        // Act
        var equal = first == second;

        // Assert
        Assert.True(equal);
        Assert.Equal(first, second);
    }
}
