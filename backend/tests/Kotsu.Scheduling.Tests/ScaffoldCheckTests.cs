namespace Kotsu.Scheduling.Tests;

public class ScaffoldCheckTests
{
    [Fact]
    public void IsReady_WhenLibraryIsReferenced_ReturnsTrue()
    {
        // Act
        var isReady = ScaffoldCheck.IsReady();

        // Assert
        Assert.True(isReady);
    }
}
