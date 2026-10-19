using Kotsu.Scheduling.Domain;

namespace Kotsu.Scheduling.Tests.Domain;

public class ProgressTests
{
    private static readonly SubTopicId _surds = SubTopicId.Create("surds").Value;
    private static readonly SubTopicId _indices = SubTopicId.Create("indices").Value;

    [Fact]
    public void Create_WithNothingComplete_IsEmpty()
    {
        // Act
        var result = Progress.Create([]);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value.Completed);
    }

    [Fact]
    public void Create_WithDuplicates_MergesThem()
    {
        // Act
        var result = Progress.Create([_surds, _surds, _indices]);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value.Completed.Count);
    }

    [Fact]
    public void IsComplete_ForAListedSubTopic_IsTrue()
    {
        // Arrange
        var progress = Progress.Create([_surds]).Value;

        // Act
        var isComplete = progress.IsComplete(_surds);

        // Assert
        Assert.True(isComplete);
    }

    [Fact]
    public void IsComplete_ForAnUnlistedSubTopic_IsFalse()
    {
        // Arrange
        var progress = Progress.Create([_surds]).Value;

        // Act
        var isComplete = progress.IsComplete(_indices);

        // Assert
        Assert.False(isComplete);
    }
}
