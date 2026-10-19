using Kotsu.Scheduling.Domain;

namespace Kotsu.Scheduling.Tests.Domain;

public class NameRulesTests
{
    public static TheoryData<Func<string?, IReadOnlyList<ValidationError>>> NamedTypes => new()
    {
        name => ExamBoard.Create(BoardId.Create("sqa").Value, name).Errors,
        name => QualificationStage.Create(StageId.Create("sqa-higher").Value, name).Errors,
        name => Subject.Create(SubjectId.Create("maths").Value, name).Errors,
    };

    [Theory]
    [MemberData(nameof(NamedTypes))]
    public void Create_WithValidName_Succeeds(Func<string?, IReadOnlyList<ValidationError>> create)
    {
        // Act
        var errors = create("National 5");

        // Assert
        Assert.Empty(errors);
    }

    [Theory]
    [MemberData(nameof(NamedTypes))]
    public void Create_WithBlankName_FailsWithBlank(Func<string?, IReadOnlyList<ValidationError>> create)
    {
        // Act
        var errors = new[] { create(null), create(""), create("   ") };

        // Assert
        Assert.All(errors, e => Assert.Equal(["name.blank"], e.Select(x => x.Code)));
    }

    [Theory]
    [MemberData(nameof(NamedTypes))]
    public void Create_WithSurroundingSpaces_FailsWithUntrimmed(Func<string?, IReadOnlyList<ValidationError>> create)
    {
        // Act
        var errors = new[] { create(" Higher"), create("Higher ") };

        // Assert
        Assert.All(errors, e => Assert.Equal(["name.untrimmed"], e.Select(x => x.Code)));
    }

    [Theory]
    [MemberData(nameof(NamedTypes))]
    public void Create_With100Characters_Succeeds(Func<string?, IReadOnlyList<ValidationError>> create)
    {
        // Act
        var errors = create(new string('a', 100));

        // Assert
        Assert.Empty(errors);
    }

    [Theory]
    [MemberData(nameof(NamedTypes))]
    public void Create_With101Characters_FailsWithTooLong(Func<string?, IReadOnlyList<ValidationError>> create)
    {
        // Act
        var errors = create(new string('a', 101));

        // Assert
        Assert.Equal(["name.too-long"], errors.Select(e => e.Code));
    }
}
