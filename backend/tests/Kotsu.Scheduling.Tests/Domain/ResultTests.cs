using Kotsu.Scheduling.Domain;

namespace Kotsu.Scheduling.Tests.Domain;

public class ResultTests
{
    [Fact]
    public void Success_HoldsValueAndNoErrors()
    {
        // Act
        var result = Result.Success(42);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(42, result.Value);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void Failure_HoldsErrorsAndNoValue()
    {
        // Arrange
        var error = new ValidationError("test.broken", "Broken.");

        // Act
        var result = Result.Failure<int>([error]);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal([error], result.Errors);
        Assert.Throws<InvalidOperationException>(() => result.Value);
    }

    [Fact]
    public void Failure_WithNoErrors_Throws()
    {
        // Act
        var act = () => Result.Failure<int>([]);

        // Assert
        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void Failure_FromTwoFailures_HoldsEveryError()
    {
        // Arrange
        var first = Result.Failure<int>([new ValidationError("test.first", "First.")]);
        var second = Result.Failure<int>([new ValidationError("test.second", "Second.")]);

        // Act
        var combined = Result.Failure<string>(first.Errors.Concat(second.Errors));

        // Assert
        Assert.Equal(["test.first", "test.second"], combined.Errors.Select(e => e.Code));
    }
}
