using Kotsu.Scheduling.Domain;

namespace Kotsu.Scheduling.Tests.Domain;

public class CountryTests
{
    private static readonly CountryId _scotland = CountryId.Create("scotland").Value;

    private static QualificationStage Stage(string code) =>
        QualificationStage.Create(StageId.Create(code).Value, code).Value;

    private static ExamBoard Board(string code) =>
        ExamBoard.Create(BoardId.Create(code).Value, code).Value;

    [Fact]
    public void Create_WithStagesAndBoards_Succeeds()
    {
        // Arrange
        QualificationStage[] stages = [Stage("sqa-national-5"), Stage("sqa-higher")];
        ExamBoard[] boards = [Board("sqa")];

        // Act
        var result = Country.Create(_scotland, "Scotland", stages, boards);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(_scotland, result.Value.Id);
        Assert.Equal("Scotland", result.Value.Name);
        Assert.Equal(stages, result.Value.Stages);
        Assert.Equal(boards, result.Value.Boards);
    }

    [Fact]
    public void Create_WithNoStages_FailsWithNoStages()
    {
        // Act
        var result = Country.Create(_scotland, "Scotland", [], [Board("sqa")]);

        // Assert
        Assert.Equal(["country.no-stages"], result.Errors.Select(e => e.Code));
    }

    [Fact]
    public void Create_WithNoBoards_FailsWithNoBoards()
    {
        // Act
        var result = Country.Create(_scotland, "Scotland", [Stage("sqa-higher")], []);

        // Assert
        Assert.Equal(["country.no-boards"], result.Errors.Select(e => e.Code));
    }

    [Fact]
    public void Create_WithDuplicateStageIds_FailsWithDuplicateStage()
    {
        // Act
        var result = Country.Create(_scotland, "Scotland", [Stage("sqa-higher"), Stage("sqa-higher")], [Board("sqa")]);

        // Assert
        Assert.Equal(["country.duplicate-stage"], result.Errors.Select(e => e.Code));
    }

    [Fact]
    public void Create_WithDuplicateBoardIds_FailsWithDuplicateBoard()
    {
        // Act
        var result = Country.Create(_scotland, "Scotland", [Stage("sqa-higher")], [Board("sqa"), Board("sqa")]);

        // Assert
        Assert.Equal(["country.duplicate-board"], result.Errors.Select(e => e.Code));
    }

    [Fact]
    public void Create_WithEveryRuleBroken_ReportsEveryError()
    {
        // Act
        var result = Country.Create(_scotland, " ", [], []);

        // Assert
        Assert.Equal(["country.no-boards", "country.no-stages", "name.blank"], result.Errors.Select(e => e.Code).Order());
    }

    [Fact]
    public void RequiresBoardChoice_WithOneBoard_IsFalse()
    {
        // Arrange
        var country = Country.Create(_scotland, "Scotland", [Stage("sqa-higher")], [Board("sqa")]).Value;

        // Act
        var requiresChoice = country.RequiresBoardChoice;

        // Assert
        Assert.False(requiresChoice);
    }

    [Fact]
    public void RequiresBoardChoice_WithTwoBoards_IsTrue()
    {
        // Arrange
        var england = CountryId.Create("england").Value;
        var country = Country.Create(england, "England", [Stage("ks4")], [Board("aqa"), Board("ocr")]).Value;

        // Act
        var requiresChoice = country.RequiresBoardChoice;

        // Assert
        Assert.True(requiresChoice);
    }
}
