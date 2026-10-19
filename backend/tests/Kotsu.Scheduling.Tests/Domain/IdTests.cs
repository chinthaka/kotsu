using Kotsu.Scheduling.Domain;

namespace Kotsu.Scheduling.Tests.Domain;

public class IdTests
{
    [Theory]
    [InlineData("scotland")]
    [InlineData("sqa-national-5")]
    [InlineData("ks4")]
    [InlineData("a")]
    public void Create_WithValidCode_Succeeds(string code)
    {
        // Act
        var result = CountryId.Create(code);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(code, result.Value.Code);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithMissingCode_FailsWithEmpty(string? code)
    {
        // Act
        var result = CountryId.Create(code);

        // Assert
        Assert.Equal(["id.empty"], result.Errors.Select(e => e.Code));
    }

    [Theory]
    [InlineData("Scotland")]
    [InlineData("north wales")]
    [InlineData("-ks4")]
    [InlineData("ks4-")]
    [InlineData("ks--4")]
    [InlineData("ks_4")]
    [InlineData("ks4!")]
    public void Create_WithBadCharacters_FailsWithInvalidFormat(string code)
    {
        // Act
        var result = CountryId.Create(code);

        // Assert
        Assert.Equal(["id.invalid-format"], result.Errors.Select(e => e.Code));
    }

    [Fact]
    public void Create_With64Characters_Succeeds()
    {
        // Arrange
        var code = new string('a', 64);

        // Act
        var result = CountryId.Create(code);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public void Create_With65Characters_FailsWithTooLong()
    {
        // Arrange
        var code = new string('a', 65);

        // Act
        var result = CountryId.Create(code);

        // Assert
        Assert.Equal(["id.too-long"], result.Errors.Select(e => e.Code));
    }

    [Fact]
    public void Create_WithTooLongAndBadCharacters_ReportsBoth()
    {
        // Arrange
        var code = new string('A', 65);

        // Act
        var result = CountryId.Create(code);

        // Assert
        Assert.Equal(["id.invalid-format", "id.too-long"], result.Errors.Select(e => e.Code).Order());
    }

    [Fact]
    public void Ids_WithSameCode_AreEqual()
    {
        // Arrange
        var first = CountryId.Create("wales").Value;

        // Act
        var second = CountryId.Create("wales").Value;

        // Assert
        Assert.Equal(first, second);
    }

    [Fact]
    public void EveryIdType_AppliesTheCodeRules()
    {
        // Act
        var invalid = new[]
        {
            CountryId.Create("Bad").IsSuccess,
            StageId.Create("Bad").IsSuccess,
            SubjectId.Create("Bad").IsSuccess,
            BoardId.Create("Bad").IsSuccess,
            TopicId.Create("Bad").IsSuccess,
            SubTopicId.Create("Bad").IsSuccess,
        };
        var valid = new[]
        {
            CountryId.Create("good").IsSuccess,
            StageId.Create("good").IsSuccess,
            SubjectId.Create("good").IsSuccess,
            BoardId.Create("good").IsSuccess,
            TopicId.Create("good").IsSuccess,
            SubTopicId.Create("good").IsSuccess,
        };

        // Assert
        Assert.All(invalid, Assert.False);
        Assert.All(valid, Assert.True);
    }
}
