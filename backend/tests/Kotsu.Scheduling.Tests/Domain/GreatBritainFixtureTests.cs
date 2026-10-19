using Kotsu.Scheduling.Domain;

namespace Kotsu.Scheduling.Tests.Domain;

/// <summary>
/// Builds the Great Britain launch countries purely as data, to show that qualification stages
/// are content rather than code. The syllabus content is illustrative, not curated (see phase 14).
/// </summary>
public class GreatBritainFixtureTests
{
    private static T Ok<T>(Result<T> result)
    {
        Assert.True(result.IsSuccess, string.Join("; ", result.Errors.Select(e => $"{e.Code}: {e.Message}")));
        return result.Value;
    }

    private static QualificationStage Stage(string code, string name) => Ok(QualificationStage.Create(Ok(StageId.Create(code)), name));

    private static ExamBoard Board(string code, string name) => Ok(ExamBoard.Create(Ok(BoardId.Create(code)), name));

    private static Result<Country> England() => Country.Create(
        Ok(CountryId.Create("england")),
        "England",
        [Stage("england-ks4", "Key Stage 4"), Stage("england-ks5", "Key Stage 5")],
        [Board("aqa", "AQA"), Board("pearson-edexcel", "Pearson Edexcel"), Board("ocr", "OCR")]);

    private static Result<Country> Scotland() => Country.Create(
        Ok(CountryId.Create("scotland")),
        "Scotland",
        [
            Stage("sqa-national-5", "National 5"),
            Stage("sqa-higher", "Higher"),
            Stage("sqa-advanced-higher", "Advanced Higher"),
        ],
        [Board("sqa", "SQA")]);

    private static Result<Country> Wales() => Country.Create(
        Ok(CountryId.Create("wales")),
        "Wales",
        [Stage("wales-ks4-gcse", "Key Stage 4 (GCSE)"), Stage("wales-post-16", "Post-16 (AS and A level)")],
        [Board("wjec", "WJEC")]);

    private static SubTopic NewSubTopic(string code, string name, decimal hours) =>
        Ok(SubTopic.Create(Ok(SubTopicId.Create(code)), name, Ok(StudyHours.Create(hours))));

    private static Topic NewTopic(string code, string name, params SubTopic[] subTopics) =>
        Ok(Topic.Create(Ok(TopicId.Create(code)), name, subTopics));

    [Fact]
    public void LaunchCountries_BuildFromDataAlone()
    {
        // Act
        Result<Country>[] countries = [England(), Scotland(), Wales()];

        // Assert
        Assert.All(countries, c => Assert.Empty(c.Errors));
        Assert.Equal(
            ["Key Stage 4", "Key Stage 5", "National 5", "Higher", "Advanced Higher", "Key Stage 4 (GCSE)", "Post-16 (AS and A level)"],
            countries.SelectMany(c => c.Value.Stages).Select(s => s.Name));
    }

    [Fact]
    public void RequiresBoardChoice_IsTrueOnlyForEngland()
    {
        // Arrange
        var england = Ok(England());
        var scotland = Ok(Scotland());
        var wales = Ok(Wales());

        // Act
        bool[] requiresChoice = [england.RequiresBoardChoice, scotland.RequiresBoardChoice, wales.RequiresBoardChoice];

        // Assert
        Assert.Equal([true, false, false], requiresChoice);
    }

    [Fact]
    public void NationalFiveSyllabus_CarriesTopicsWithDerivedHours()
    {
        // Arrange
        var scotland = Ok(Scotland());
        var nationalFive = scotland.Stages.Single(s => s.Name == "National 5");
        var sqa = scotland.Boards.Single();
        var mathematics = Ok(Subject.Create(Ok(SubjectId.Create("mathematics")), "Mathematics"));

        // Act
        var syllabus = Syllabus.Create(nationalFive.Id, mathematics.Id, sqa.Id,
        [
            NewTopic("expressions-and-formulae", "Expressions and formulae",
                NewSubTopic("surds-and-indices", "Surds and indices", 2m),
                NewSubTopic("algebraic-fractions", "Algebraic fractions", 1.5m)),
            NewTopic("relationships", "Relationships",
                NewSubTopic("linear-equations", "Linear equations", 1m),
                NewSubTopic("quadratic-functions", "Quadratic functions", 2.5m),
                NewSubTopic("trigonometry", "Trigonometry", 2m)),
        ]);

        // Assert
        Assert.Empty(syllabus.Errors);
        Assert.Equal([3.5m, 5.5m], syllabus.Value.Topics.Select(t => t.EstimatedHours.Value));
    }
}
